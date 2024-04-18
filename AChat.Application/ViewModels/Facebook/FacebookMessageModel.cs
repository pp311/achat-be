using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AChat.Application.ViewModels.Facebook;

public class FacebookMessageModel
{
	[JsonProperty("sender")]
	public FacebookSender? Sender { get; set; }
	
	[JsonProperty("recipient")]
	public FacebookRecipient? Recipient { get; set; }
	
	[JsonProperty("timestamp")]
	public long Timestamp { get; set; }
	
	[JsonProperty("message")]
	public FacebookMessage? Message { get; set; }
}

public class FacebookSender
{
	[JsonProperty("id")]
	public string? Id { get; set; }
}

public class FacebookRecipient
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }
}

public class FacebookMessage
{
	[JsonProperty("mid")]
	public string? Mid { get; set; }
	[JsonProperty("text")]
	public string? Text { get; set; }

	public List<FacebookMessageAttachment> Attachments { get; set; } = new();
}

public class FacebookMessageAttachment
{
	[JsonProperty("type")]
	public string? Type { get; set; }
	
	[JsonProperty("payload")]
	public FacebookMessagePayload? Payload { get; set; }
}

public class FacebookMessagePayload
{
	[JsonProperty("url")]
	public string? Url { get; set; }
}

public class FacebookWrapper
{
	[JsonProperty("entry")] 
	public List<FacebookMessageWrapper> Entry { get; set; } = new();
}

public class FacebookMessageWrapper
{
	[JsonProperty("id")] 
	public string? Id { get; set; }
	
	[JsonProperty("time")]
	public long Time { get; set; }

	[JsonProperty("messaging")] 
	public List<FacebookMessageModel> Messaging { get; set; } = new();
}
