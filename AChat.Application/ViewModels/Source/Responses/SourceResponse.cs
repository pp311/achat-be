using AChat.Domain;
using AutoMapper;

namespace AChat.Application.ViewModels.Source.Responses;

public class SourceResponse
{
    public int Id { get; set; }
    public SourceType Type { get; set; }
    
    public string? Name { get; set; }
}

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Entities.Source, SourceResponse>();
    }
}