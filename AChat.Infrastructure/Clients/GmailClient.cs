using System.Text;
using System.Text.RegularExpressions;
using AChat.Application.Common.Configurations;
using AChat.Application.Common.Dtos;
using AChat.Application.Common.Interfaces;
using AChat.Domain.Entities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Message = Google.Apis.Gmail.v1.Data.Message;

namespace AChat.Infrastructure.Clients;

public class GmailClient : IGmailClient
{
    private readonly GoogleSettings _googleSettings;
    private readonly ILogger<GmailClient> _logger;

    public GmailClient(IOptions<GoogleSettings> googleSettings, ILogger<GmailClient> logger)
    {
        _logger = logger;
        _googleSettings = googleSettings.Value;
    }

    public async Task<(string AccessToken, string RefreshToken)> GetCredentialFromCodeAsync(string code)
    {
        var token = await GetFlow().ExchangeCodeForTokenAsync("me", code, "http://localhost:5173", CancellationToken.None);

        return (token.AccessToken, token.RefreshToken);
    }

    public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
    {
        var token = await GetFlow().RefreshTokenAsync(string.Empty, refreshToken, CancellationToken.None);

        return (token.AccessToken, token.RefreshToken);
    }
    
    public UserCredential GetUserCredentialAsync(string accessToken, string refreshToken)
    {
        var token = new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return new UserCredential(GetFlow(), string.Empty, token);
    }
    
    public async Task<List<GmailDto>> GetEmailsAsync(UserCredential credential)
    {
        var service = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential
        });
        
        var result = new List<GmailDto>();

        var request = service.Users.Messages.List("me");
        var listMessagesResponse = await request.ExecuteAsync();
        
        foreach (var message in listMessagesResponse.Messages)
        {
            var messageRequest = service.Users.Messages.Get("me", message.Id);
            Message messageResponse = await messageRequest.ExecuteAsync();
            
            var payload = messageResponse.Payload;
            var threadId = messageResponse.ThreadId;
            
            var headers = payload.Headers;
            
            var subject = headers?.FirstOrDefault(_ => _.Name == "Subject")?.Value;
            var from = headers?.FirstOrDefault(_ => _.Name == "From")?.Value;
            var to = headers?.FirstOrDefault(_ => _.Name == "To")?.Value;
            
            var parts = payload.Parts;
            
            // get email content
            var body = parts?.FirstOrDefault()?.Body?.Data;
            if (body != null)
            {
                var data = body.Replace("-", "+").Replace("_", "/");
                var decodedData = Convert.FromBase64String(data);
                var text = Encoding.UTF8.GetString(decodedData);
                
                if (subject != null && from != null && to != null)
                {
                    var email = new GmailDto
                    {
                        Subject = subject,
                        From = from,
                        To = to,
                        Content = text,
                        ThreadId = threadId
                    };
                    
                    result.Add(email);
                }
                
            }


            // var attachments = parts?.Where(_ => !string.IsNullOrEmpty(_.Filename)).ToList();
            // foreach (var attachment in attachments ?? [])
            // {
            //     var attachmentId = attachment.Body.AttachmentId;
            //     var attachmentRequest = service.Users.Messages.Attachments.Get("me", message.Id, attachmentId);
            //     var attachmentResponse = await attachmentRequest.ExecuteAsync();
            //     
            //     var attachmentData = attachmentResponse.Data;
            //     var attachmentDecodedData = Convert.FromBase64String(attachmentData);
            // }
        }
        
        return result;
    }
    
    public async Task<Userinfo?> GetInfoAsync(UserCredential credential)
    {
        var oauthService = new Oauth2Service(
        new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential
        });

        return await oauthService.Userinfo.Get().ExecuteAsync();
    }
    
    public async Task<ulong> GetHistoryIdAsync(UserCredential credential)
    {
        var service = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential
        });

        var request = service.Users.GetProfile("me");
        request.Fields = "historyId";
        var profile = await request.ExecuteAsync();
        
        return profile.HistoryId ?? 0;
    }
    
    public async Task SubscribeAsync(UserCredential credential)
    {
        var service = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential
        });

        var request = service.Users.Watch(new WatchRequest
        {
            TopicName = _googleSettings.Topic
        }, "me");

        await request.ExecuteAsync();
    }
    
    public async Task UnsubscribeAsync(UserCredential credential)
    {
        var service = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential
        });

        var request = service.Users.Stop("me");

        await request.ExecuteAsync();
    }
    
    public async Task<List<GmailDto>> GetMessageAsync(UserCredential credential, ulong historyId, string labelId = "INBOX")
    {
        var service = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential
        });

        // Get inbox and sent emails
        var request = service.Users.History.List("me");
        request.StartHistoryId = historyId;
        request.LabelId = labelId;
        var listHistoryResponse = await request.ExecuteAsync();
        
        var result = new List<GmailDto>();
        _logger.LogInformation($"List history count: {listHistoryResponse.History?.Count}");
        
        foreach (var history in listHistoryResponse.History ?? [])
        {
            foreach (var message in history.Messages)
            {
                var messageRequest = service.Users.Messages.Get("me", message.Id);
                var messageResponse = await messageRequest.ExecuteAsync();
                
                var payload = messageResponse!.Payload;
                var threadId = messageResponse.ThreadId;
                
                var headers = payload.Headers;
                
                var subject = headers?.FirstOrDefault(_ => _.Name == "Subject")?.Value;
                var from = headers?.FirstOrDefault(_ => _.Name == "From")?.Value;
                var to = headers?.FirstOrDefault(_ => _.Name == "To")?.Value;
                var messageId = headers?.FirstOrDefault(_ => _.Name == "Message-ID")?.Value;
                var snippet = messageResponse.Snippet;
                
                var parts = payload.Parts;
                
                // get email content
                var body = parts?.FirstOrDefault()?.Body?.Data;
                if (body != null)
                {
                    var data = body.Replace("-", "+").Replace("_", "/");
                    var decodedData = Convert.FromBase64String(data);
                    var text = Encoding.UTF8.GetString(decodedData);
                    
                    // Remove the replied using regex
                    // For Gmail: "On {date}, {name} <{email}> wrote: " and anything after that
                    // For Outlook: "___________________________ ..."
                    text = Regex.Replace(text, @"\s*\bOn\b.*wrote:.[\s\S]*$", string.Empty);
                    text = Regex.Replace(text, @"_{20,}.*", string.Empty);
                    
                    if (subject != null && from != null && to != null)
                    {
                        var email = new GmailDto
                        {
                            Id = messageId ?? string.Empty,
                            Subject = subject,
                            // get email in the format "Name <email>"
                            From = from.Contains("<") ? from.Split("<")[1].TrimEnd('>') : from,
                            To = to.Contains("<") ? to.Split("<")[1].TrimEnd('>') : to,
                            Content = text.Trim(),
                            ThreadId = threadId,
                            FromName = from.Contains("<") ? from.Split("<")[0].Trim() : from,
                            ToName = to.Contains("<") ? to.Split("<")[0].Trim() : to,
                            Snippet = snippet
                        };
                        
                        result.Add(email);
                    }
                }
            }
        }
        
        return result;
    }
    
    public async Task<Domain.Entities.Message> SendGmailAsync(UserCredential credential, string from, string to, string subject, string body, string? replyMId = default, string? threadId = default)
    {
        var service = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential
        });
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(MailboxAddress.Parse(from));
        mimeMessage.To.Add(MailboxAddress.Parse(to));
        mimeMessage.Subject = subject;
        mimeMessage.Body = new TextPart("plain")
        {
            Text = body
        };
        
        if (threadId != default)
        {
            // var replyId = await GetThreadLastMessageIdAsync(credential, threadId);
            mimeMessage.Subject = "Re: " + subject.Replace("Re: ", "");
            mimeMessage.InReplyTo = replyMId;
            mimeMessage.References.Add(replyMId);
        }
        
        Message message = new Message();
        
        using (var memory = new MemoryStream())
        {
            await mimeMessage.WriteToAsync(memory);

            var buffer = memory.GetBuffer();
            int length = (int)memory.Length;

            message.Raw = Convert.ToBase64String(buffer, 0, length);
            
            if (threadId != default)
            {
                message.ThreadId = threadId;
            }
        }

        var request = service.Users.Messages.Send(message, "me");
        var sentMessage = await request.ExecuteAsync();

        var getMessage = await service.Users.Messages.Get("me", sentMessage.Id).ExecuteAsync();
        var messageId = getMessage.Payload.Headers.FirstOrDefault(_ => _.Name == "Message-Id")?.Value ?? string.Empty;

        return new Domain.Entities.Message
        {
            Content = body,
            Subject = subject,
            MId = messageId,
            IsEcho = true,
            ThreadId = sentMessage.ThreadId,
            Attachments = new List<MessageAttachment>(),
            IsRead = false
        };
    }
    
    public async Task<Message> GetMessageAsync(UserCredential credential, string messageId)
    {
        var service = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential
        });

        var request = service.Users.Messages.Get("me", messageId);
        return await request.ExecuteAsync();
    }
    
    public async Task<string> GetThreadLastMessageIdAsync(UserCredential credential, string threadId)
    {
        var service = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential
        });

        var request = service.Users.Threads.Get("me", threadId);
        var thread = await request.ExecuteAsync();
        
        var firstMessageId = thread.Messages?.LastOrDefault()?.Id;
        
        if (!string.IsNullOrEmpty(firstMessageId))
            return (await GetMessageAsync(credential, firstMessageId)).Payload.Headers.FirstOrDefault(_ => _.Name == "Message-ID")?.Value ?? string.Empty;
        
        return string.Empty;
    }
    
    private GoogleAuthorizationCodeFlow GetFlow()
    {
        return new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _googleSettings.ClientId, 
                ClientSecret = _googleSettings.ClientSecret,
            },
            Scopes = _googleSettings.Scopes,
            IncludeGrantedScopes = true
        });
    }
}