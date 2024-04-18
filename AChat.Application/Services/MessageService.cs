using AChat.Application.Common.Interfaces;
using AChat.Application.ViewModels.Facebook;
using AChat.Domain.Entities;
using AChat.Domain.Exceptions;
using AChat.Domain.Repositories.Base;
using AutoMapper;
using Message = AChat.Domain.Entities.Message;

namespace AChat.Application.Services;

public class MessageService(
    IUnitOfWork unitOfWork, 
    ICurrentUser currentUser, 
    IMapper mapper,
    IRepositoryBase<Source> sourceRepository, 
    IRepositoryBase<Contact> contactRepository,
    IFacebookClient facebookClient) 
    : BaseService(unitOfWork, mapper, currentUser)
{
    public async Task ReceiveFacebookMessageAsync(FacebookMessageModel message)
    {
        var source = await sourceRepository.GetAsync(_ => _.PageId == message.Recipient!.Id && _.UserId == CurrentUser.Id)
            ?? throw new NotFoundException(nameof(Source), message.Recipient!.Id);

        var contact = await contactRepository.GetAsync(_ => _.SourceId == source.Id && _.FacebookUserId == message.Sender!.Id);
        
        if (contact == null)
        {
            var profileInfo = await facebookClient.GetUserProfileInfoAsync(source.AccessToken, message.Sender!.Id)
                ?? throw new Exception("Failed to get user profile info");
            
            contact = new Contact
            {
                SourceId = source.Id,
                FacebookUserId = message.Sender!.Id,
                Name = profileInfo.Name
            };
            
            contactRepository.Add(contact);
        }
        
        var newMessage = new Message
        {
            ContactId = contact.Id,
            Attachments = message.Message!.Attachments.Select(_ => new MessageAttachment
            {
                Url = _.Payload!.Url!,
                FileName = string.Empty,
                Type = _.Type
            }).ToList(),
            Content = message.Message!.Text ?? string.Empty,
        };
        
        contact.Messages.Add(newMessage);

        await UnitOfWork.SaveChangesAsync();
    } 
    
    public async Task SendFacebookMessageAsync(int contactId, string message)
    {
        var contact = await contactRepository.GetAsync(_ => _.Id == contactId) 
            ?? throw new NotFoundException(nameof(Contact), contactId.ToString());
        
        var source = await sourceRepository.GetAsync(_ => _.Id == contact.SourceId) 
            ?? throw new NotFoundException(nameof(Source), contact.SourceId.ToString());
        
        await facebookClient.SendMessageAsync(source.AccessToken, contact.FacebookUserId!, source.PageId!, message);
    }
}