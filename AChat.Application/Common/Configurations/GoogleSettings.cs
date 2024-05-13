namespace AChat.Application.Common.Configurations;

public class GoogleSettings
{
	public string ClientId { get; set; }
	public string ClientSecret { get; set; }
	public string RedirectUri { get; set; }
	public string[] Scopes { get; set; }
	public string Topic { get; set; }
	public string BaseUrl { get; set; }
}