using System.Text.Json;
using AChat.Application.Services;
using AChat.Application.ViewModels;
using AChat.Application.ViewModels.Message;
using AChat.SignalRHub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AChat.Controllers;

[ApiController]
[Route("api/messages")]
public class MessageController : ControllerBase
{
    private readonly MessageService _messageService;
    private readonly IHubContext<SignalRHub.SignalRHub, ISignalRClient> _hubContext;

    public MessageController(MessageService messageService, IHubContext<SignalRHub.SignalRHub, ISignalRClient> hubContext)
    {
        _messageService = messageService;
        _hubContext = hubContext;
    }

    [HttpPost("facebook")]
    public async Task<IActionResult> Post(SendFacebookMessageRequest request)
    {
        await _messageService.SendFacebookMessageAsync(request);
        return Ok();
    }

    [HttpGet("facebook")]
    public async Task<IActionResult> Get([FromQuery] int contactId, [FromQuery] PagingRequest request)
    {
        var messages = await _messageService.GetFacebookMessagesAsync(contactId, request);
        return Ok(messages);
    }

    [HttpPost("gmail")]
    public async Task<IActionResult> Post(SendGmailMessageRequest request)
    {
        var responses = await _messageService.SendGmailMessageAsync(request);

        foreach (var response in responses)
        {
            await _hubContext.Clients.User(response.UserId.ToString()).ReceiveMessage(JsonSerializer.Serialize(response));
        }
        return Ok();
    }

    [HttpGet("gmail")]
    public async Task<IActionResult> GetThreads([FromQuery] int contactId, [FromQuery] PagingRequest request)
    {
        var messages = await _messageService.GetGmailThreadsAsync(contactId, request);
        return Ok(messages);
    }

    [HttpGet("gmail/contacts/{contactId:int}/threads/{threadId}")]
    public async Task<IActionResult> GetThreadMessages([FromRoute] string threadId, [FromRoute] int contactId)
    {
        var messages = await _messageService.GetThreadMessagesAsync(contactId, threadId);
        return Ok(messages);
    }

    [HttpDelete("gmail/threads")]
    public async Task<IActionResult> DeleteThreads([FromQuery] int contactId, [FromQuery] List<string> threadIds)
    {
        await _messageService.DeleteGmailThreadsAsync(contactId, threadIds);
        return Ok();
    }

    [HttpPost("mark-read")]
    public async Task<IActionResult> MarkRead([FromQuery] int contactId, int messageId, string? threadId)
    {
        await _messageService.MarkReadAsync(contactId, messageId, threadId);
        return Ok();
    }
}
