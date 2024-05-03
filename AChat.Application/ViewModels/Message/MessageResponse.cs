using System.Text.Json.Serialization;
using AChat.Domain.Entities;
using AutoMapper;

namespace AChat.Application.ViewModels.Message;

public class MessageResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }
    [JsonPropertyName("mId")]
    public string? MId { get; set; }
    [JsonPropertyName("isEcho")]
    public bool IsEcho { get; set; }
    [JsonPropertyName("contactId")]
    public int ContactId { get; set; }
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
    [JsonPropertyName("attachments")]
    public ICollection<MessageAttachmentResponse> Attachments { get; set; } = new List<MessageAttachmentResponse>();
    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }
    [JsonPropertyName("updatedOn")]
    public DateTime? UpdatedOn { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;
    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public class MessageAttachmentResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;
    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Entities.Message, MessageResponse>();
        CreateMap<MessageAttachment, MessageAttachmentResponse>();
    }
}
