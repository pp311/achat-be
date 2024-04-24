namespace AChat.Application.ViewModels.Contact;

public class HideContactsRequest : IRequest
{
    public List<int> ContactIds { get; set; } = new();
}