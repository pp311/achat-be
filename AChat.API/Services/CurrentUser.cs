using System.Security.Claims;
using AChat.Application.Common.Interfaces;

namespace AChat.Services;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int Id => int.TryParse(_httpContextAccessor.HttpContext
        ?.User
        .FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
    
    public string? Name => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
    public string? Role => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
    public string? Email => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
    public string? AvatarUrl => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Uri);
}
