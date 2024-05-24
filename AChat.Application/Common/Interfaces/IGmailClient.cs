using AChat.Application.Common.Dtos;
using AChat.Domain.Entities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2.Data;

namespace AChat.Application.Common.Interfaces;

public interface IGmailClient
{
    Task<(string AccessToken, string RefreshToken)> GetCredentialFromCodeAsync(string code);
    Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken);
    UserCredential GetUserCredentialAsync(string accessToken, string refreshToken);

    Task<ulong> GetHistoryIdAsync(UserCredential credential);
    Task<List<GmailDto>> GetEmailsAsync(UserCredential credential);
    Task<Domain.Entities.Message> SendGmailAsync(UserCredential credential, string from, string to, string subject, string body, string? replyMId = default, string? threadId = default);
    Task<List<GmailDto>> GetMessageAsync(UserCredential credential, ulong historyId, string labelId = "INBOX");
    Task<Userinfo?> GetInfoAsync(UserCredential credential);
    Task SubscribeAsync(UserCredential credential);
    Task UnsubscribeAsync(UserCredential credential);
    Task DeleteThreadsAsync(UserCredential credential, List<string> threadIds);
}