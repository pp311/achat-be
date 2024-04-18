namespace AChat.Application.Common.Interfaces;

public interface ICurrentUser
{
    public int Id { get; }
    public string? Name { get; }
    public string? Email { get; }
    public string? Role { get; }
    public string? AvatarUrl { get; }
}