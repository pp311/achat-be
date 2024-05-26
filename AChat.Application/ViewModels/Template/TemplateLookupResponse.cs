using AutoMapper;

namespace AChat.Application.ViewModels.Template;

public class TemplateLookupResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Entities.Template, TemplateResponse>();
        CreateMap<Domain.Entities.Template, TemplateLookupResponse>();
    }
}