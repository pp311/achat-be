using System.Text;
using System.Text.Json;
using AChat.Application.Common.Dtos;
using AChat.Application.Common.Extensions;
using AChat.Application.Common.Interfaces;
using AChat.Application.ViewModels;
using AChat.Application.ViewModels.Facebook;
using AChat.Application.ViewModels.Message;
using AChat.Domain;
using AChat.Domain.Entities;
using AChat.Domain.Exceptions;
using AChat.Domain.Repositories.Base;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Message = AChat.Domain.Entities.Message;

namespace AChat.Application.Services;

public class MessageService(
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IMapper mapper,
    IRepositoryBase<Source> sourceRepository,
    IRepositoryBase<Contact> contactRepository,
    IRepositoryBase<Message> messageRepository,
    IFacebookClient facebookClient,
    IGmailClient gmailClient,
    ILogger<MessageService> logger)
    : BaseService(unitOfWork, mapper, currentUser)
{
    public async Task<List<MessageResponse>> ReceiveFacebookMessageAsync(FacebookMessageModel message)
    {
        if (message.Message!.IsEcho)
            (message.Recipient!.Id, message.Sender!.Id) = (message.Sender.Id, message.Recipient.Id);

        //Sender => contact
        //Recipient => user

        var sources = await sourceRepository.GetListAsync(_ => _.PageId == message.Recipient!.Id);
        if (!sources.Any())
            throw new NotFoundException(nameof(Source), message.Recipient!.Id);

        var responses = new List<MessageResponse>();

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
                IsEcho = message.Message!.IsEcho,
                ContactId = contact.Id,
                Attachments = message.Message!.Attachments.Select(_ => new MessageAttachment
                {
                    Url = _.Payload!.Url!,
                    FileName = string.Empty,
                    Type = _.Type
                }).ToList(),
                Content = message.Message!.Text ?? string.Empty,
                UpdatedOn = DateTime.UtcNow
            };

            contact.Messages.Add(newMessage);

            var response = Mapper.Map<MessageResponse>(newMessage);
            response.UserId = source.UserId;
            responses.Add(response);
        }

        await UnitOfWork.SaveChangesAsync();
        return responses;
    }

    public async Task SendFacebookMessageAsync(SendFacebookMessageRequest request)
    {
        var contact = await contactRepository.GetAsync(_ => _.Id == request.ContactId && _.UserId == CurrentUser.Id)
            ?? throw new NotFoundException(nameof(Contact), request.ContactId.ToString());

        var source = await sourceRepository.GetAsync(_ => _.Id == contact.SourceId && _.UserId == CurrentUser.Id)
            ?? throw new NotFoundException(nameof(Source), contact.SourceId.ToString());

        await facebookClient.SendMessageAsync(source.AccessToken, contact.FacebookUserId!, source.PageId!, request.Message, request.AttachmentUrl, request.AttachmentType);
    }
    
    // public async Task<UploadFacebookAttachmentResponse> UploadFacebookAttachmentAsync(MemoryStream stream)
    // {
    //     // var source = await sourceRepository.GetAsync(_ => _.Id == request.SourceId && _.UserId == CurrentUser.Id)
    //     //     ?? throw new NotFoundException(nameof(Source), request.SourceId.ToString());
    //     //
    //     // var attachmentUrl = await facebookClient.UploadAttachmentAsync(source.AccessToken, request.AttachmentUrl, request.AttachmentType);
    //     // return new UploadFacebookAttachmentResponse { Url = attachmentUrl };
    // }
    
    public async Task<List<MessageResponse>> GetFacebookMessagesAsync(int contactId, PagingRequest request)
    {
        var isContactExists = await contactRepository.AnyAsync(_ => _.Id == contactId && _.Source.Type == SourceType.Facebook);
        if (!isContactExists)
            throw new NotFoundException(nameof(Contact), contactId.ToString());

        // Hack: PageNumber is message id 
        return await messageRepository.GetQuery(_ => _.ContactId == contactId)
            .WhereIf(request.PageNumber > 0, _ => _.Id < request.PageNumber)
            .OrderByDescending(_ => _.CreatedOn)
            .Take(request.PageSize)
            .ProjectToListAsync<MessageResponse>(Mapper.ConfigurationProvider);
    }
    
    public async Task ReceiveGmailAsync(GoogleWebhookDto message)
    {
        var data = Encoding.UTF8.GetString(Convert.FromBase64String(message.Message.Data)); 
        var webhookData = JsonSerializer.Deserialize<GoogleWebhookData>(data);
        
        if (webhookData == null)
        {
            logger.LogError("Failed to deserialize Google webhook data");
            return;
        }

        var source = await sourceRepository.GetAsync(_ => _.Name == webhookData.EmailAddress && _.Type == SourceType.Gmail);
        
        if (source == null)
        {
            logger.LogError("Failed to find Gmail source");
            return;
        }
        
        var messages = new List<GmailDto>();

        try
        {
            var credential = gmailClient.GetUserCredentialAsync(source.AccessToken!, source.RefreshToken!);
            messages = await gmailClient.GetMessageAsync(credential, source.HistoryId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get Gmail messages");
        }
        
        logger.LogInformation("Received {Count} Gmail messages", messages.Count);
        
        foreach (var gmailDto in messages)
        {
            if (await messageRepository.AnyAsync(_ => _.MId == gmailDto.Id))
                continue;
            
            var isEcho = gmailDto.From == source.Name;
            var contactMail = isEcho ? gmailDto.To : gmailDto.From;
            var contact = await contactRepository.GetAsync(_ => _.SourceId == source.Id && _.Email == contactMail);
            
            if (contact == null)
            {
                contact = new Contact
                {
                    SourceId = source.Id,
                    Email = gmailDto.From,
                    UserId = source.UserId,
                    Name = isEcho ? gmailDto.ToName : gmailDto.FromName
                };
                
                contactRepository.Add(contact);
            }

            var newMessage = new Message
            {
                MId = gmailDto.Id,
                ContactId = contact.Id,
                Content = gmailDto.Content,
                Subject = gmailDto.Subject,
                ThreadId = gmailDto.ThreadId,
                IsEcho = isEcho
            };
            
            contact.Messages.Add(newMessage);
        }
        
        source.HistoryId = webhookData.HistoryId;
        
        await UnitOfWork.SaveChangesAsync();
    }
    
    public async Task SendGmailMessageAsync(SendGmailMessageRequest request)
    {
        var contact = await contactRepository.GetAsync(_ => _.Id == request.ContactId)
            ?? throw new NotFoundException(nameof(Contact), request.ContactId.ToString());

        var source = await sourceRepository.GetAsync(_ => _.Id == contact.SourceId)
            ?? throw new NotFoundException(nameof(Source), contact.SourceId.ToString());

        var credential = gmailClient.GetUserCredentialAsync(source.AccessToken!, source.RefreshToken!);
        
        var from = string.IsNullOrEmpty(source.Name)
            ? source.Email!
            : $"{source.Name} <{source.Email}>";

        if (request.ReplyMessageId > 0)
        {
            var replyMessage = await messageRepository.GetAsync(_ => _.Id == request.ReplyMessageId)
                               ?? throw new NotFoundException(nameof(Message), request.ReplyMessageId.ToString());

            await gmailClient.SendGmailAsync(credential, from, contact.Email!, request.Subject, request.Message, replyMessage.ThreadId);
        }
        else
            await gmailClient.SendGmailAsync(credential, from ,contact.Email!, request.Subject, request.Message);
    }
}
