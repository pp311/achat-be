using AChat.Application.Common.Extensions;
using AChat.Application.Common.Interfaces;
using AChat.Application.Common.Models;
using AChat.Application.ViewModels.Contact;
using AChat.Domain.Entities;
using AChat.Domain.Exceptions;
using AChat.Domain.Repositories;
using AChat.Domain.Repositories.Base;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AChat.Application.Services;

public class ContactService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUser currentUser,
    IRepositoryBase<Source> sourceRepository,
    IGmailClient gmailClient,
    IContactRepository contactRepository) : BaseService(unitOfWork, mapper, currentUser)
{
    public async Task<PaginatedList<ContactResponse>> GetContactsAsync(GetContactsRequest request, CancellationToken ct)
    {
        return await contactRepository.Search(request.Search)
            .Where(_ => _.UserId == CurrentUser.Id && _.IsHidden == request.IsHidden)
            .WhereIf(request.TagIds.Any(), _ => _.Tags.Any(tag => request.TagIds.Contains(tag.Id)))
            .WhereIf(request.SourceIds.Any(), _ => request.SourceIds.Contains(_.SourceId))
            .WhereIf(request.Type.HasValue, _ => _.Source.Type == request.Type)
            .OrderBy(GetOrderByField(request.SortBy), request.IsDescending)
            .ProjectTo<ContactResponse>(Mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);
    }

    public async Task<ContactResponse> GetContactAsync(int id, CancellationToken ct)
    {
        return await contactRepository.GetQuery()
            .Where(_ => _.Id == id && _.UserId == CurrentUser.Id)
            .ProjectTo<ContactResponse>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct) ?? throw new NotFoundException(nameof(Contact), id.ToString());
    }

    public async Task CreateContactAsync(CreateContactRequest request, CancellationToken ct)
    {
        var source = await sourceRepository.GetQuery()
            .FirstOrDefaultAsync(_ => _.Id == request.SourceId && _.UserId == CurrentUser.Id, ct)
            ?? throw new NotFoundException(nameof(Source), request.SourceId.ToString());

        var isContactExists = await contactRepository.GetQuery()
            .AnyAsync(_ => _.Email == request.Email && _.SourceId == request.SourceId && _.UserId == CurrentUser.Id, ct);

        if (isContactExists)
            throw new AppException("Contact already exists");

        // Profile profile;
        // try {
        //     var credential = gmailClient.GetUserCredentialAsync(source.AccessToken!, source.RefreshToken!);
        //     profile = await gmailClient.GetProfileAsync(credential, request.Email);
        // } catch (Exception e) {
        //     throw new AppException("Failed to get profile", e);
        // }

        var contact = new Contact
        {
            Email = request.Email,
            Name = request.Email,
            SourceId = request.SourceId,
            UserId = CurrentUser.Id
        };

        contactRepository.Add(contact);
        await UnitOfWork.SaveChangesAsync(ct);
    }

    public async Task SetRefContactAsync(int contactId, int refId)
    {
        var contact = await contactRepository
            .GetAsync(_ => _.Id == contactId && _.UserId == CurrentUser.Id)
            ?? throw new NotFoundException(nameof(Contact), contactId.ToString());

        if (refId != 0)
        {
            var refContact = await contactRepository
                .GetAsync(_ => _.Id == refId && _.UserId == CurrentUser.Id)
                ?? throw new NotFoundException(nameof(Contact), contactId.ToString());

            refContact.RefId = contactId;
            contactRepository.Update(refContact);
        }

        if (contact.RefId != 0)
        {
            var oldRefContact = await contactRepository
                .GetAsync(_ => _.Id == contact.RefId && _.UserId == CurrentUser.Id)
                ?? throw new NotFoundException(nameof(Contact), contactId.ToString());

            oldRefContact.RefId = 0;
            contactRepository.Update(oldRefContact);
        }

        contact.RefId = refId;
        contactRepository.Update(contact);
        await UnitOfWork.SaveChangesAsync();
    }

    public async Task ChangeContactsVisibilityAsync(List<int> id, bool visibility, CancellationToken ct)
    {
        var contacts = await contactRepository.GetListAsync(_ => id.Contains(_.Id) && _.UserId == CurrentUser.Id, ct);
        foreach (var contact in contacts)
        {
            contact.IsHidden = !visibility;
            contactRepository.Update(contact);
        }
        await UnitOfWork.SaveChangesAsync(ct);
    }

    private static IOrderByField GetOrderByField(ContactSortByOption? option)
    {
        return option switch
        {
            ContactSortByOption.Id
                => new OrderByField<Contact, int>(x => x.Id),
            ContactSortByOption.Name
                => new OrderByField<Contact, string>(x => x.Name!),
            ContactSortByOption.LastMessage
                => new OrderByField<Contact, DateTime>(x => x.Messages.Max(_ => _.CreatedOn).Value),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };
    }
}
