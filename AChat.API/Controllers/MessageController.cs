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
        await _messageService.SendGmailMessageAsync(request);
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
    
    [HttpPost("mark-read")]
    public async Task<IActionResult> MarkRead([FromQuery] int contactId, int messageId)
    {
        await _messageService.MarkReadAsync(contactId, messageId);
        return Ok();
    }
}
