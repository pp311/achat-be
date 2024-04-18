using AChat.Application.Services;
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
    public async Task<IActionResult> Post([FromQuery] string message, int contactId)
    {
        await _messageService.SendFacebookMessageAsync(contactId, message);
        return Ok();
    }
}