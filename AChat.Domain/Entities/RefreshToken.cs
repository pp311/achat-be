using AChat.Domain.Entities.Base;

namespace AChat.Domain.Entities;

public class RefreshToken : EntityBase
{
    public string Token { get; set; } = null!;
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}