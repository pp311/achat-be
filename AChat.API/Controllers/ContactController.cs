using AChat.Application.Services;
using AChat.Application.ViewModels.Contact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AChat.Controllers;

[ApiController]
[Route("api/contacts")]
[Authorize]
public class ContactController : ControllerBase
{
    private readonly ContactService _contactService;

    public ContactController(ContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<IActionResult> GetContacts([FromQuery] GetContactsRequest request, CancellationToken ct)
    {
        var result = await _contactService.GetContactsAsync(request, ct);
        return Ok(result);
    } 
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetContact([FromRoute] int id, CancellationToken ct)
    {
        var result = await _contactService.GetContactAsync(id, ct);
        return Ok(result);
    }
    
    [HttpPost("hide")]
    public async Task<IActionResult> HideContacts([FromBody] ChangeContactsVisibilityRequest request, CancellationToken ct)
    {
        await _contactService.ChangeContactsVisibilityAsync(request.ContactIds, false, ct);
        return NoContent();
    }
    
    [HttpPost("un-hide")]
    public async Task<IActionResult> UnHideContacts([FromBody] ChangeContactsVisibilityRequest request, CancellationToken ct)
    {
        await _contactService.ChangeContactsVisibilityAsync(request.ContactIds, true, ct);
        return NoContent();
    }
}