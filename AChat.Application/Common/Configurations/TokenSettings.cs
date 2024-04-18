namespace AChat.Application.Common.Configurations;

public class TokenSettings
{
    public string Key { get; set; } = null!;
    public int ExpiryInMinutes { get; set; }
    public int RefreshTokenExpiryInHours { get; set; }
}