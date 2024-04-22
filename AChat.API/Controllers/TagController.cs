using AChat.Application.Services;
using AChat.Application.ViewModels.Tag.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AChat.Controllers;

[ApiController]
[Route("api/tags")]
public class TagController : ControllerBase
{
    private readonly TagService _tagService;

    public TagController(TagService tagService)
    {
        _tagService = tagService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetTagsRequest request)
    {
        var tags = await _tagService.GetTagsAsync(request);
        return Ok(tags);
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromQuery] string name, [FromQuery] string? color)
    {
        await _tagService.AddTagAsync(name, color);
        return Ok();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _tagService.DeleteTagAsync(id);
        return Ok();
    }
    
    [HttpGet("contact/{contactId}")]
    public async Task<IActionResult> GetTagsOfContact(int contactId)
    {
        var tags = await _tagService.GetTagsOfContactAsync(contactId);
        return Ok(tags);
    }
    
    [HttpDelete("{tagId}/contact/{contactId}")]
    public async Task<IActionResult> RemoveTagFromContact(int contactId, int tagId)
    {
        await _tagService.RemoveTagFromContactAsync(tagId, contactId);
        return Ok();
    }
    
    [HttpPost("{tagId}/contact/{contactId}")]
    public async Task<IActionResult> AddTagToContact(int contactId, int tagId)
    {
        await _tagService.AddTagToContactAsync(tagId, contactId);
        return Ok();
    }
}