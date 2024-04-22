
using System.Text.Json.Serialization;

namespace AChat.Application.ViewModels.Tag.Requests;

public class GetTagsRequest : PagingRequest
{
    public string Search { get; set; } = string.Empty; 
    
    public TagSortByOption SortBy { get; set; } = TagSortByOption.Id;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TagSortByOption
{
    Id,
    Name,
}