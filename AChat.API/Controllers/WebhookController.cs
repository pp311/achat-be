using System.Text;
using AChat.Application.Common.Configurations;
using AChat.Application.Common.Dtos;
using AChat.Application.Services;
using AChat.Application.ViewModels.Facebook;
using AChat.SignalRHub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AChat.Controllers;

[ApiController]
[Route("api/webhook")]
public class WebhookController : ControllerBase
{
    private readonly ILogger<WebhookController> _logger;
    private readonly FacebookSettings _facebookSettings;
    private readonly MessageService _messageService;
    private readonly IHubContext<SignalRHub.SignalRHub, ISignalRClient> _hubContext;

    public WebhookController(ILogger<WebhookController> logger,
        IOptions<FacebookSettings> facebookSettings,
        MessageService messageService,
        IHubContext<SignalRHub.SignalRHub, ISignalRClient> hubContext)
    {
        _logger = logger;
        _messageService = messageService;
        _hubContext = hubContext;
        _facebookSettings = facebookSettings.Value;
    }

    [HttpGet("facebook")]
    public string Get([FromQuery(Name = "hub.mode")] string hubMode,
                      [FromQuery(Name = "hub.challenge")] string hubChallenge,
                      [FromQuery(Name = "hub.verify_token")] string hubVerifyToken)
    {
        return hubVerifyToken == _facebookSettings.ValidateSecret ? hubChallenge : string.Empty;
    }

    [HttpPost("facebook")]
    public async Task<IActionResult> Post([FromBody] FacebookWrapper request)
    {
        // _logger.LogInformation($"Received message from {request.Entry.First().Messaging.First().Sender.Id}: {request.Entry.First().Messaging.First().Message.Text}");
        foreach (var messaging in request.Entry.SelectMany(entry => entry.Messaging))
        {
            var responses = await _messageService.ReceiveFacebookMessageAsync(messaging);
            foreach (var response in responses)
            {
                await _hubContext.Clients.User(response.UserId.ToString()).ReceiveMessage(JsonSerializer.Serialize(response));
            }
        }

        return Ok();
    }

    [HttpPost("google")]
    public async Task<IActionResult> PostGoogle([FromBody] GoogleWebhookDto request)
    {
        await HandleGoogleWebhook(request);

        return Ok();
    }

    private async Task HandleGoogleWebhook(GoogleWebhookDto request)
    {
        var responses = await _messageService.ReceiveGmailAsync(request);

        foreach (var response in responses)
        {
            await _hubContext.Clients.User(response.UserId.ToString()).ReceiveMessage(JsonSerializer.Serialize(response));
        }
    }

    [HttpGet("send")]
    public async Task<IActionResult> Post([FromQuery] string text,
                                          [FromQuery] string accessToken,
                                          [FromQuery] string receiverId)
    {
        HttpClient client = new HttpClient();
        var body = new
        {
            messaging_type = "RESPONSE",
            recipient = new
            {
                id = receiverId
            },
            message = new
            {
                text = text
            }
        };


        var url = $"https://graph.facebook.com/v12.0/me/messages?access_token={accessToken}";

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

        using StringContent jsonContent = new(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json");

        await client.PostAsync(url, jsonContent);

        return Ok();
    }
}

