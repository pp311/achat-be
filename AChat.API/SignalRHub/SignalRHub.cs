using AChat.Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace AChat.SignalRHub;

public class SignalRHub : Hub
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<SignalRHub> _logger;

    public SignalRHub(IMemoryCache memoryCache, ILogger<SignalRHub> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }
    
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"New connection: {Context.ConnectionId}");
        // if (string.IsNullOrEmpty(Context.UserIdentifier))
        // await _memoryCache.GetOrCreateAsync(CacheHelper.GetSignalRUserKey(Context.ConnectionId, Context.UserIdentifier!), entry =>
        // {
        //     entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
        //     return Task.FromResult(Context.ConnectionId);
        // });
    } 
}