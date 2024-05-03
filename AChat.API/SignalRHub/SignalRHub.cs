using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace AChat.SignalRHub;

[Authorize]
public class SignalRHub : Hub<ISignalRClient>
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<SignalRHub> _logger;
    private static readonly ConcurrentDictionary<int, List<string>> ConnectedUsers = new();

    public SignalRHub(IMemoryCache memoryCache, ILogger<SignalRHub> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }
    
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"New connection: {Context.ConnectionId} {Context.UserIdentifier}");
        var userId = int.TryParse(Context.UserIdentifier, out var id) ? id : 0;
        // Try to get a List of existing user connections from the cache
        ConnectedUsers.TryGetValue(userId, out var existingUserConnectionIds);

        // happens on the very first connection from the user
        existingUserConnectionIds ??= new List<string>();

        existingUserConnectionIds.Add(Context.ConnectionId);

        ConnectedUsers.TryAdd(userId, existingUserConnectionIds);

        await base.OnConnectedAsync();
    } 
}

public interface ISignalRClient
{
    Task ReceiveMessage(string message);
}