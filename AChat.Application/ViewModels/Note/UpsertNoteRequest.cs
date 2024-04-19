namespace AChat.Application.ViewModels.Note;

public class UpsertNoteRequest
{
    public string Content { get; set; } = string.Empty; 
    public int ContactId { get; set; }
}