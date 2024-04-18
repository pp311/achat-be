using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AChat.Application.ViewModels.Facebook;

public class FacebookInfoModel
{
	[JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = null!;
    
    public string Id { get; set; } = null!;
    
    public string? Name { get; set; }
    
    public FacebookAccountModel? Accounts { get; set; }
    public FacebookErrorModel? Error { get; set; }
}

public class FacebookAccountModel
{
	public List<FacebookAccountDataModel> Data { get; set; } = new();
}

public class FacebookAccountDataModel
{
	public string Id { get; set; } = null!;
	public string Name { get; set; } = null!;
	
	[JsonPropertyName("access_token")]
	public string AccessToken { get; set; } = null!;
}

public class FacebookAccessToken
{
	[JsonPropertyName("access_token")]
	public string AccessToken { get; set; } = null!;
}