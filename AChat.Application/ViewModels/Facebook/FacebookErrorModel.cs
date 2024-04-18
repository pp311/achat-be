using Newtonsoft.Json;

namespace AChat.Application.ViewModels.Facebook;

public class FacebookErrorModel
{
    public FacebookErrorDetailModel Error { get; set; } = null!;
}

public class FacebookErrorDetailModel
{
    public string Message { get; set; } = null!;
    public string Type { get; set; } = null!;
    public int Code { get; set; }
    [JsonProperty("error_subcode")]
    public string? ErrorSubcode { get; set; }
}