using System.Text.Json.Serialization;

namespace AChat.Application.ViewModels.Facebook;

public class FacebookSendMessageModel
{
    [JsonPropertyName("messaging_type")]
    public string? MessagingType { get; set; } = "RESPONSE";
    
    [JsonPropertyName("message")]
    public FacebookSendMessage? Message { get; set; }
    [JsonPropertyName("recipient")]
    public FacebookRecipient? Recipient { get; set; }
    
}

public class FacebookSendMessage
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
    
    [JsonPropertyName("attachment")]
    public FacebookSendMessageAttachment? Attachment { get; set; }
}

public class FacebookSendMessageAttachment
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    [JsonPropertyName("payload")]
    public FacebookSendMessageAttachmentPayload? Payload { get; set; }
}

public class FacebookSendMessageAttachmentPayload
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
    
    [JsonPropertyName("attachment_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AttachmentId { get; set; }
    
    [JsonPropertyName("is_reusable")]
    public bool IsReusable { get; set; }
}