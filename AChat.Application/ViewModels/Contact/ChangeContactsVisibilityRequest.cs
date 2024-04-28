namespace AChat.Application.ViewModels.Contact;

public class ChangeContactsVisibilityRequest : IRequest
{
    public List<int> ContactIds { get; set; } = new();
}