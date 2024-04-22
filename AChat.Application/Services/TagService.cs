using AChat.Application.Common.Extensions;
using AChat.Application.Common.Interfaces;
using AChat.Application.Common.Models;
using AChat.Application.ViewModels.Tag.Requests;
using AChat.Application.ViewModels.Tag.Responses;
using AChat.Domain.Entities;
using AChat.Domain.Exceptions;
using AChat.Domain.Repositories.Base;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AChat.Application.Services;

public class TagService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUser currentUser,
    IRepositoryBase<Tag> tagRepository,
    IRepositoryBase<Contact> contactRepository) : BaseService(unitOfWork, mapper, currentUser)
{
    public async Task AddTagAsync(string name, string? color)
    {
        if (await tagRepository.AnyAsync(_ => _.Name == name && _.UserId == CurrentUser.Id))
            throw new AlreadyExistsException(nameof(Tag), name);
        
        var tag = new Tag
        {
            Name = name,
            UserId = CurrentUser.Id,
            Color = color ?? GetRandomColor()
        };

        tagRepository.Add(tag);
        await UnitOfWork.SaveChangesAsync();
    }
    
    public async Task DeleteTagAsync(int id)
    {
        var tag = await tagRepository.GetAsync(_ => _.Id == id && _.UserId == CurrentUser.Id)
            ?? throw new NotFoundException(nameof(Tag), id.ToString());
        
        tagRepository.Delete(tag);
        await UnitOfWork.SaveChangesAsync();
    }
    
    public async Task<PaginatedList<TagResponse>> GetTagsAsync(GetTagsRequest request)
    {
        return await tagRepository
            .GetQuery(_ => _.UserId == CurrentUser.Id 
                           && (string.IsNullOrEmpty(request.Search) || _.Name.Contains(request.Search)))
            .OrderBy(GetOrderByField(request.SortBy), request.IsDescending)
            .ToPaginatedListAsync<TagResponse>(Mapper.ConfigurationProvider, request.PageNumber, request.PageSize);
    }
    
    public async Task<List<TagResponse>> GetTagsOfContactAsync(int contactId)
    {
        var tags = await tagRepository.GetListAsync(_ => _.UserId == CurrentUser.Id && _.Contacts.Any(ct => ct.Id == contactId));
        return Mapper.Map<List<TagResponse>>(tags);
    }
    
    public async Task RemoveTagFromContactAsync(int tagId, int contactId)
    {
        var tag = await tagRepository.GetQuery(_ => _.Id == tagId && _.UserId == CurrentUser.Id)
                .Include(_ => _.Contacts)
                .FirstOrDefaultAsync()
            ?? throw new NotFoundException(nameof(Tag), tagId.ToString());
        
        var contact = await contactRepository.GetByIdAsync(contactId)
            ?? throw new NotFoundException(nameof(Contact), contactId.ToString());
        
        tag.Contacts.Remove(contact);
        await UnitOfWork.SaveChangesAsync();
    }
    
    public async Task AddTagToContactAsync(int tagId, int contactId)
    {
        var tag = await tagRepository.GetQuery(_ => _.Id == tagId && _.UserId == CurrentUser.Id)
                .Include(_ => _.Contacts)
                .FirstOrDefaultAsync()
            ?? throw new NotFoundException(nameof(Tag), tagId.ToString());
        
        var contact = await contactRepository.GetByIdAsync(contactId)
            ?? throw new NotFoundException(nameof(Contact), contactId.ToString());
        
        if (tag.Contacts.Any(_ => _.Id == contactId))
            throw new AlreadyExistsException(nameof(Contact), contactId.ToString());
        
        tag.Contacts.Add(contact);
        await UnitOfWork.SaveChangesAsync();
    }

    #region Helper
    private static string GetRandomColor()
    {
        var random = new Random();
        return $"#{random.Next(0x1000000):X6}";
    }
    
    private static IOrderByField GetOrderByField(TagSortByOption? option)
    {
        return option switch
        {
            TagSortByOption.Id
                => new OrderByField<Tag, int>(x => x.Id),
            TagSortByOption.Name
                => new OrderByField<Tag, string>(x => x.Name),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };
    }

    #endregion
}
