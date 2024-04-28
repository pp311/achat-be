
using System.Text.Json.Serialization;

namespace AChat.Application.ViewModels.Tag.Requests;

public class GetTagsRequest : PagingRequest
{
    public string? Search { get; set; }
    public List<int> ExcludeIds { get; set; } = new();
 
    public TagSortByOption SortBy { get; set; } = TagSortByOption.Id;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TagSortByOption
{
    Id,
    Name,
}
