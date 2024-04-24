using System.Text.Json.Serialization;
using AChat.Domain;

namespace AChat.Application.ViewModels.Contact;

public class GetContactsRequest : PagingRequest
{
    public string? Search { get; set; }
    
    public ContactSortByOption SortBy { get; set; } = ContactSortByOption.Id;
    public List<int> TagIds { get; set; } = new();
    public SourceType? Type { get; set; }
    public bool IsHidden { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ContactSortByOption
{
    Id,
    Name,
}
