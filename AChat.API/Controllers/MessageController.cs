using AChat.Application.Services;
using AChat.Application.ViewModels;
using AChat.Application.ViewModels.Message;
using Microsoft.AspNetCore.Mvc;

namespace AChat.Controllers;

[ApiController]
[Route("api/messages")]
public class MessageController : ControllerBase
{
    private readonly MessageService _messageService;

    public MessageController(MessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost("facebook")]
    public async Task<IActionResult> Post(SendFacebookMessageRequest request)
    {
        await _messageService.SendFacebookMessageAsync(request.ContactId, request.Message);
        return Ok();
    }

    [HttpGet("facebook")]
    public async Task<IActionResult> Get([FromQuery] int contactId, [FromQuery] PagingRequest request)
    {
        var messages = await _messageService.GetFacebookMessagesAsync(contactId, request);
        return Ok(messages);
    }
}
