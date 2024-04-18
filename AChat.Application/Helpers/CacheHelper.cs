namespace AChat.Application.Helpers;

public static class CacheHelper
{
    public static string GetSignalRUserKey(string connectionId, string userId) => $"signalr_user_{connectionId}_{userId}"; 
}