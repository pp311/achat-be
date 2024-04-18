using System.Text;
using System.Text.Json;
using AChat.Application.Common.Configurations;
using AChat.Application.Common.Interfaces;
using AChat.Application.ViewModels.Facebook;
using AChat.Domain.Exceptions;
using Microsoft.Extensions.Options;
using RestSharp;

namespace AChat.Infrastructure.Clients;

public class FacebookClient(IOptions<FacebookSettings> settings) : IFacebookClient
{
    private readonly FacebookSettings _settings = settings.Value;

    public async Task<FacebookInfoModel?> GetPageInfoAsync(string accessToken)
    {
        var client = new RestClient(_settings.BaseUrl + "/me");
        var request = new RestRequest()
            .AddParameter("access_token", accessToken)
            .AddParameter("fields", "id,name,accounts");
        
        return await client.GetAsync<FacebookInfoModel>(request);
    }

    public async Task<FacebookAccountModel?> GetPageLongLiveTokenAsync(string accessToken, string userId)
    {
        var client = new RestClient(_settings.BaseUrl + "/oauth/access_token");
        var request = new RestRequest()
            .AddParameter("grant_type", "fb_exchange_token")
            .AddParameter("client_id", _settings.AppId)
            .AddParameter("client_secret", _settings.AppSecret)
            .AddParameter("fb_exchange_token", accessToken);
        
        var userLongLiveToken = await client.GetAsync<FacebookAccessToken>(request);
        
        if (string.IsNullOrEmpty(userLongLiveToken?.AccessToken))
        {
            throw new Exception("Failed to get long live token");
        }
        
        client = new RestClient(_settings.BaseUrl + $"/{userId}/accounts");
        request = new RestRequest()
            .AddParameter("access_token", userLongLiveToken.AccessToken);
        
        return await client.GetAsync<FacebookAccountModel>(request);
    }
    
    public async Task SubscribeAppAsync(string accessToken, string pageId)
    {
        var client = new RestClient(_settings.BaseUrl + $"/{pageId}/subscribed_apps");
        
        var request = new RestRequest()
            .AddParameter("access_token", accessToken)
            .AddParameter("subscribed_fields", "messages");
        
        var response = await client.PostAsync(request);
        if (!response.IsSuccessful)
            throw new AppException("Failed to subscribe app to page");
    }
    
    public async Task UnsubscribeAppsAsync(string accessToken, List<string> pageIds)
    {
        foreach (var pageId in pageIds)
        {
            var client = new RestClient(_settings.BaseUrl + $"/{pageId}/subscribed_apps");
            
            var request = new RestRequest()
                .AddParameter("access_token", accessToken);
            
            await client.DeleteAsync(request);
        }
    }

    public async Task<FacebookInfoModel?> GetUserProfileInfoAsync(string? accessToken, string? userId)
    {
        var client = new RestClient(_settings.BaseUrl + $"/{userId}");
        var request = new RestRequest()
            .AddParameter("access_token", accessToken)
            .AddParameter("fields", "id,name");
        
        return await client.GetAsync<FacebookInfoModel>(request);
    }
    
    public async Task SendMessageAsync(string? accessToken, string receiverId, string pageId, string message)
    {
        var client = new RestClient();
        var body = new FacebookSendMessageModel
        {
            Message = new FacebookSendMessage
            {
                Text = message
            },
            Recipient = new FacebookRecipient
            {
                Id = receiverId
            }
        };
        
        var request = new RestRequest(_settings.BaseUrl + $"/{pageId}/messages?access_token={accessToken}", Method.Post)
            .AddJsonBody(body);
        
        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
            throw new AppException("Failed to send message");
    }
}