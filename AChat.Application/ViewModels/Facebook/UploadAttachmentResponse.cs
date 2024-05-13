using System.Text.Json.Serialization;

namespace AChat.Application.ViewModels.Facebook;

public class UploadAttachmentResponse
{
    [JsonPropertyName("attachment_id")]
    public string? AttachmentId { get; set; }
}