using AutoMapper;

namespace AChat.Application.ViewModels.Tag.Responses;

public class TagResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Entities.Tag, TagResponse>();
    }
}