using AChat.Domain;
using AutoMapper;

namespace AChat.Application.ViewModels.Contact;

public class ContactResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public SourceType SourceType { get; set; }
}

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Entities.Contact, ContactResponse>()
            .ForMember(dest => dest.SourceType, opt => opt.MapFrom(src => src.Source.Type));
    }
}