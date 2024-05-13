using AChat.Domain;

namespace AChat.Application.ViewModels.Message;

public class SendFacebookMessageRequest
{
    public string Message { get; set; } = null!;
    public string? AttachmentUrl { get; set; }
    public int ContactId { get; set; }
    public string? AttachmentType { get; set; }
}
