using System.Text.Json.Serialization;

namespace AChat.Application.ViewModels.Contact;

public class GetContactsRequest : PagingRequest
{
    public string? Search { get; set; }
    
    public ContactSortByOption SortBy { get; set; } = ContactSortByOption.Id;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ContactSortByOption
{
    Id,
    Name,
}
