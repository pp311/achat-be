using AChat.Application.Common.Extensions;
using AChat.Application.Common.Interfaces;
using AChat.Application.Common.Models;
using AChat.Application.ViewModels.Contact;
using AChat.Domain.Entities;
using AChat.Domain.Repositories;
using AChat.Domain.Repositories.Base;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace AChat.Application.Services;

public class ContactService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUser currentUser,
    IContactRepository contactRepository) : BaseService(unitOfWork, mapper, currentUser)
{
    public async Task<PaginatedList<ContactResponse>> GetContactsAsync(GetContactsRequest request, CancellationToken ct)
    {
        var contacts = await contactRepository.Search(request.Search)
            .Where(_ => _.UserId == currentUser.Id)
            .OrderBy(GetOrderByField(request.SortBy), request.IsDescending)
            .ProjectTo<ContactResponse>(Mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);

        return contacts;
    }
    
    private static IOrderByField GetOrderByField(ContactSortByOption? option)
    {
        return option switch
        {
            ContactSortByOption.Id
                => new OrderByField<Contact, int>(x => x.Id),
            ContactSortByOption.Name
                => new OrderByField<Contact, string>(x => x.Name!),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };
    }
}