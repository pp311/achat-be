namespace AChat.Application.Common.Dtos;

public class GmailDto
{
    public string Id { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string ThreadId { get; set; } = null!;
    public string? ReplyTo { get; set; }
    public string From { get; set; } = null!;
    public string To { get; set; } = null!;
    public string? FromName { get; set; }
    public string? ToName { get; set; }
    public string? Snippet { get; set; }
}