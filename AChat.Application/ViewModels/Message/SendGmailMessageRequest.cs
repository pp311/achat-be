namespace AChat.Application.ViewModels.Message;

public class SendGmailMessageRequest
{
    public int ContactId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public int ReplyMessageId { get; set; }

    public List<MessageAttachmentResponse> Attachments { get; set; } = new();
}