using AutoMapper;

namespace AChat.Application.ViewModels.Note;

public class NoteResponse
{
    public int Id { get; set; } 
    public string Content { get; set; } = string.Empty;
    
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Entities.Note, NoteResponse>();
    }
}