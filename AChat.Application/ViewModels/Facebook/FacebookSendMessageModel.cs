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
}