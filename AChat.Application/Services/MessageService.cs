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
        if (message.Message!.IsEcho)
            (message.Recipient!.Id, message.Sender!.Id) = (message.Sender.Id, message.Recipient.Id);
        
        var sources = await sourceRepository.GetListAsync(_ => _.PageId == message.Recipient!.Id);
        
        foreach (var source in sources)
        {
            var contact = await contactRepository.GetAsync(_ => _.SourceId == source.Id && _.FacebookUserId == message.Sender!.Id);
            
            if (contact == null)
            {
                try
                {
                    var profileInfo = await facebookClient.GetUserProfileInfoAsync(source.AccessToken, message.Sender!.Id);
                    if (profileInfo == null)
                        continue;
                    
                    contact = new Contact
                    {
                        SourceId = source.Id,
                        FacebookUserId = message.Sender!.Id,
                        Name = profileInfo.Name,
                        UserId = source.UserId
                    };

                    contactRepository.Add(contact);
                }
                catch
                {
                    continue;
                }
            }

            var newMessage = new Message
            {
                MId = message.Message!.Mid,
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
        }

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