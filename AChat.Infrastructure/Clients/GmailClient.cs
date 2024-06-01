using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using AChat.Application.Common.Configurations;
using AChat.Application.Common.Dtos;
using AChat.Application.Common.Interfaces;
using AChat.Application.ViewModels.Message;
using AChat.Domain.Entities;
using AChat.Domain.Exceptions;
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
using Minio;
using Minio.DataModel.Args;
using Message = Google.Apis.Gmail.v1.Data.Message;

namespace AChat.Infrastructure.Clients;

public class GmailClient : IGmailClient
{
    private readonly GoogleSettings _googleSettings;
    private readonly ILogger<GmailClient> _logger;
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _minioSettings;

    public GmailClient(IOptions<GoogleSettings> googleSettings, ILogger<GmailClient> logger, IMinioClient minioClient, IOptions<MinioSettings> minioSettings)
    {
        _logger = logger;
        _minioClient = minioClient;
        _minioSettings = minioSettings.Value;
        _googleSettings = googleSettings.Value;
    }

    public async Task<(string AccessToken, string RefreshToken)> GetCredentialFromCodeAsync(string code)
    {
        var token = await GetFlow().ExchangeCodeForTokenAsync("me", code, _googleSettings.RedirectUri, CancellationToken.None);

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

    public async Task<Userinfo?> GetInfoAsync(UserCredential credential)
    {
        var oauthService = new Oauth2Service(
        new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential
        });

        return await oauthService.Userinfo.Get().ExecuteAsync();
    }
    
    public async Task<Profile> GetProfileAsync(UserCredential credential, string email)
    {
        var service = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential
        });

        var request = service.Users.GetProfile(email);
        return await request.ExecuteAsync();
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
                Message? messageResponse = null;
                try
                {
                    var messageRequest = service.Users.Messages.Get("me", message.Id);
                    messageResponse = await messageRequest.ExecuteAsync();
                    if (messageResponse == null) continue;
                }
                catch { }

                var payload = messageResponse?.Payload;
                var threadId = messageResponse?.ThreadId;

                if (payload == null) continue;

                var headers = payload.Headers;

                var subject = headers?.FirstOrDefault(_ => _.Name == "Subject")?.Value;
                var from = headers?.FirstOrDefault(_ => _.Name == "From")?.Value;
                var to = headers?.FirstOrDefault(_ => _.Name == "To")?.Value;
                var messageId = headers?.FirstOrDefault(_ => _.Name == "Message-ID")?.Value;
                var replyTo = headers?.FirstOrDefault(_ => _.Name == "In-Reply-To")?.Value;
                var snippet = messageResponse?.Snippet ?? string.Empty;
                var attachments = payload.Parts?
                    .Where(_ => !string.IsNullOrEmpty(_.Filename) 
                                && _.Headers.FirstOrDefault(_ => _.Name == "Content-Disposition")?.Value.Contains("inline") != true)
                    .ToList();
                var inlineImages = payload.Parts?
                    .Where(_ => !string.IsNullOrEmpty(_.Filename) 
                                && _.Headers.FirstOrDefault(_ => _.Name == "Content-Disposition")?.Value.Contains("inline") == true)
                    .ToList();

                var parts = payload.Parts;

                // get email content
                string? body;
                string? htmlBody;
                // attachments & inline images
                if (parts?.Any(_ => _.MimeType == "multipart/related") == true)
                {
                    parts = parts?.Where(_ => _.MimeType == "multipart/related").FirstOrDefault()?.Parts;
                    
                    inlineImages = parts?.Where(_ => !string.IsNullOrEmpty(_.Filename) 
                                                    && _.Headers.FirstOrDefault(_ => _.Name == "Content-Disposition")?.Value.Contains("inline") == true)
                                        .ToList();
                    
                    var contentParts = parts?.Where(_ => _.MimeType == "multipart/alternative").FirstOrDefault()?.Parts;

                    body = contentParts?.FirstOrDefault()?.Body?.Data;
                    htmlBody = contentParts?.Where(_ => _.MimeType == "text/html").FirstOrDefault()?.Body?.Data;
                }
                else if (parts?.Any(_ => _.MimeType == "multipart/alternative") == true)
                {
                    parts = parts?.Where(_ => _.MimeType == "multipart/alternative").FirstOrDefault()?.Parts;

                    body = parts?.FirstOrDefault()?.Body?.Data;
                    htmlBody = parts?.Where(_ => _.MimeType == "text/html").FirstOrDefault()?.Body?.Data;
                }
                else
                {
                    body = parts?.FirstOrDefault()?.Body?.Data;
                    htmlBody = parts?.Where(_ => _.MimeType == "text/html").FirstOrDefault()?.Body?.Data;
                }
                
                body = !string.IsNullOrEmpty(htmlBody) ? htmlBody : body;
                if (body != null || attachments?.Any() == true)
                {
                    var data = body?.Replace("-", "+").Replace("_", "/") ?? string.Empty;
                    var decodedData = Convert.FromBase64String(data);
                    var text = Encoding.UTF8.GetString(decodedData);

                    // Remove the replied using regex
                    // For Gmail: "On {date}, {name} <{email}> wrote: " and anything after that
                    // For Outlook: "___________________________ ..."
                    text = Regex.Replace(text, @"\s*\bOn\b.*wrote:.[\s\S]*$", string.Empty);
                    text = Regex.Replace(text, @"_{20,}.*", string.Empty);
                    
                    // replace inline images
                    foreach (var part in inlineImages ?? [])
                    {
                        if (part.Filename != null && part.Body.AttachmentId != null)
                        {
                            var attachmentId = part.Body.AttachmentId;
                            var cid = part.Headers.FirstOrDefault(_ => _.Name == "X-Attachment-Id")?.Value;

                            if (text.Contains($"cid:{cid}"))
                            {
                                var attachmentRequest = service.Users.Messages.Attachments.Get("me", message.Id, attachmentId);
                                var attachmentResponse = await attachmentRequest.ExecuteAsync();

                                var attachmentData = attachmentResponse.Data.Replace("-", "+").Replace("_", "/");
                                var attachmentDecodedData = Convert.FromBase64String(attachmentData);

                                text = text.Replace($"cid:{cid}", await TestMinio(attachmentDecodedData, part.Filename));
                            }
                        }
                    }

                    var attachmentList = new List<MessageAttachment>();
                    foreach (var attachment in attachments ?? [])
                    {
                        try
                        {
                            var attachmentId = attachment.Body.AttachmentId;
                            var attachmentRequest = service.Users.Messages.Attachments.Get("me", message.Id, attachmentId);
                            var attachmentResponse = await attachmentRequest.ExecuteAsync();

                            var attachmentData = attachmentResponse.Data.Replace("-", "+").Replace("_", "/");
                            var attachmentDecodedData = Convert.FromBase64String(attachmentData);

                            attachmentList.Add(new MessageAttachment
                            {
                                FileName = attachment.Filename,
                                Url = await TestMinio(attachmentDecodedData, attachment.Filename)
                            });
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e.Message);
                        }
                    }

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
                            ThreadId = threadId ?? string.Empty,
                            FromName = from.Contains("<") ? from.Split("<")[0].Trim() : from,
                            ToName = to.Contains("<") ? to.Split("<")[0].Trim() : to,
                            Snippet = snippet,
                            ReplyTo = replyTo,
                            Attachments = attachmentList
                        };

                        result.Add(email);
                    }
                }
            }
        }

        return result;
    }

    public async Task<Domain.Entities.Message> SendGmailAsync(UserCredential credential, 
        string from, string to, string subject, string body, string? replyMId = default, string? threadId = default, List<MessageAttachmentResponse>? attachments = null)
    {
        var service = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential
        });
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(MailboxAddress.Parse(from));
        mimeMessage.To.Add(MailboxAddress.Parse(to));
        mimeMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = body
        };
        
        // Todo: parallel this
        foreach (var attachment in attachments ?? [])
        {
            var memoryStream = await GetBase64FromS3Async(attachment.FileName!);
            var byteContent = memoryStream.ToArray();
            bodyBuilder.Attachments.Add(attachment.FileName, byteContent);
        }
        

        mimeMessage.Body = bodyBuilder.ToMessageBody();

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

    public async Task DeleteThreadsAsync(UserCredential credential, List<string> threadIds)
    {
        var service = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential
        });

        foreach (var threadId in threadIds)
        {
            var request = service.Users.Threads.Delete("me", threadId);
            await request.ExecuteAsync();
        }
    }

    private async Task<string> TestMinio(byte[] data, string fileName)
    {
        if (!await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_minioSettings.BucketName)))
            throw new AppException("Bucket does not exist");
        using var newMemoryStream = new MemoryStream(data);

        var size = newMemoryStream.Length;
        newMemoryStream.Position = 0;
        var args = new PutObjectArgs()
            .WithBucket(_minioSettings.BucketName)
            .WithObject($"{fileName}")
            .WithObjectSize(size)
            .WithStreamData(newMemoryStream);

        await _minioClient.PutObjectAsync(args);

        return $"{_minioSettings.BaseUrl}/{fileName}";
    }

    private async Task<MemoryStream> GetBase64FromS3Async(string fileName)
    {
        if (!await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_minioSettings.BucketName)))
            throw new AppException("Bucket does not exist");
        
        var newMemoryStream = new MemoryStream();
        
        var args = new GetObjectArgs()
            .WithBucket(_minioSettings.BucketName)
            .WithObject($"{fileName}")
            .WithCallbackStream(str => str.CopyTo(newMemoryStream));
        
        await _minioClient.GetObjectAsync(args);
        
        return newMemoryStream; 
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
