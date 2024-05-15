using AChat.Application.ViewModels.Facebook;
using AChat.Domain;

namespace AChat.Application.Common.Interfaces;

public interface IFacebookClient
{
    Task<FacebookInfoModel?> GetPageInfoAsync(string accessToken);
    Task<FacebookProfilePicture?> GetProfilePictureAsync(string? accessToken, string? userId);
    Task<FacebookAccountModel?> GetPageLongLiveTokenAsync(string accessToken, string userId);
    Task SubscribeAppAsync(string accessToken, string pageId);
    Task UnsubscribeAppsAsync(string? accessToken, List<string> pageIds);
    Task<FacebookInfoModel?> GetUserProfileInfoAsync(string? accessToken, string? userId);
    Task SendMessageAsync(string? accessToken, string receiverId, string pageId, string? message, string? attachmentUrl = null, string? fileType = null);
}