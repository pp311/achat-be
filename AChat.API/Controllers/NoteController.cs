using AChat.Application.Services;
using AChat.Application.ViewModels.Note;
using Microsoft.AspNetCore.Mvc;

namespace AChat.Controllers;

[ApiController]
[Route("api/notes")]
public class NoteController : ControllerBase
{
    private readonly NoteService _noteService;

    public NoteController(NoteService noteService)
    {
        _noteService = noteService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetNotes([FromQuery] int contactId, CancellationToken ct)
    {
        var result = await _noteService.GetNotesAsync(contactId, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNote([FromBody] UpsertNoteRequest request, CancellationToken ct)
    {
        return Ok(await _noteService.CreateNoteAsync(request, ct));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] UpsertNoteRequest request, CancellationToken ct)
    {
        await _noteService.UpdateNoteAsync(id, request, ct);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(int id, CancellationToken ct)
    {
        await _noteService.DeleteNoteAsync(id, ct);
        return NoContent();
    } 
}