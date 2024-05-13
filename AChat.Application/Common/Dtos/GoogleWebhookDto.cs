using System.Text.Json.Serialization;

namespace AChat.Application.Common.Dtos;

public class GoogleWebhookDto
{
    public GoogleWebhookMessage Message { get; set; }
}

public class GoogleWebhookMessage
{
    public string Data { get; set; }
    public string MessageId { get; set; }
    public string PublishTime { get; set; }
}

public class GoogleWebhookData
{
    [JsonPropertyName("emailAddress")]
    public string EmailAddress { get; set; }
    [JsonPropertyName("historyId")]
    public ulong HistoryId { get; set; }
}