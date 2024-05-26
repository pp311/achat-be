using AChat.Application.Services;
using AChat.Application.ViewModels.Template;
using AChat.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AChat.Controllers;

[ApiController]
[Route("api/templates")]
public class TemplateController : ControllerBase
{
    private readonly TemplateService _templateService;

    public TemplateController(TemplateService templateService)
    {
        _templateService = templateService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var templates = await _templateService.GetTemplatesAsync(ct);
        return Ok(templates);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var template = await _templateService.GetTemplateAsync(id, ct);
        return Ok(template);
    }
    
    [HttpGet("lookup")]
    public async Task<IActionResult> Get([FromQuery] TemplateType? type, CancellationToken ct)
    {
        var templates = await _templateService.GetTemplateLookupAsync(type, ct);
        return Ok(templates);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateTemplateRequest request, CancellationToken ct)
    {
        var id = await _templateService.CreateTemplateAsync(request, ct);
        return Ok(id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateTemplateRequest request, CancellationToken ct)
    {
        await _templateService.UpdateTemplateAsync(id, request, ct);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _templateService.DeleteTemplateAsync(id, ct);
        return Ok();
    } 
}