using AChat.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AChat.Controllers;

[ApiController]
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
}