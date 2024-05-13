namespace AChat.Application.Common.Dtos;

public class GmailDto
{
    public string Id { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public string ThreadId { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string FromName { get; set; }
    public string ToName { get; set; }
}