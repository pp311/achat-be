using Microsoft.AspNetCore.Identity;

namespace AChat.Domain.Entities;

public class User : IdentityUser<int>
{
    public string? AvatarUrl { get; set; }
    public string FullName { get; set; } = string.Empty;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}