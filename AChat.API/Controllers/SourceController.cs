using AChat.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AChat.Controllers;

[ApiController]
[Authorize]
[Route("api/sources")]
public class SourceController : ControllerBase
{
    private readonly SourceService _sourceService;

    public SourceController(SourceService sourceService)
    {
        _sourceService = sourceService;
    }
    
    [HttpPost("connect-facebook")]
    public async Task ConnectFacebook([FromQuery] string accessToken)
    {
        await _sourceService.ConnectFacebookAsync(accessToken);
    }
    
    [HttpPost("disconnect-facebook")]
    public async Task DisconnectFacebook([FromQuery] string pageId)
    {
        await _sourceService.DisconnectFacebookAsync(pageId);
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var sources = await _sourceService.GetSourcesAsync();
        return Ok(sources);
    }
    
    [HttpPost("connect-google")]
    public async Task ConnectGoogle([FromQuery] string code)
    {
        await _sourceService.ConnectGmailAsync(code);
    }
    
    [HttpPost("disconnect-google")]
    public async Task DisconnectGoogle([FromQuery] int sourceId)
    {
        await _sourceService.DisconnectGmailAsync(sourceId);
    }
}