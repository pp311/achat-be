namespace AChat.Application.ViewModels.Message;

public class SendFacebookMessageRequest
{
    public int ContactId { get; set; }
    public string Message { get; set; } = null!;
}
