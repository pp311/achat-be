using AChat.Application.Common.Interfaces;
using AChat.Application.ViewModels.Tag.Responses;
using AChat.Domain.Entities;
using AChat.Domain.Exceptions;
using AChat.Domain.Repositories.Base;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AChat.Application.Services;

public class TagService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUser currentUser,
    IRepositoryBase<Tag> tagRepository,
    IRepositoryBase<Contact> contactRepository) : BaseService(unitOfWork, mapper, currentUser)
{
    public async Task AddTagAsync(string name)
    {
        if (await tagRepository.AnyAsync(_ => _.Name == name && _.UserId == CurrentUser.Id))
            throw new AlreadyExistsException(nameof(Tag), name);
        
        var tag = new Tag
        {
            Name = name,
            UserId = CurrentUser.Id
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
    
    public async Task<List<TagResponse>> GetTagsAsync()
    {
        var tags = await tagRepository.GetListAsync(_ => _.UserId == CurrentUser.Id);
        return Mapper.Map<List<TagResponse>>(tags);
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
}
