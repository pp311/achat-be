using AutoMapper;

namespace AChat.Application.ViewModels.Message;

public class GetGmailThreadResponse
{
    public string Id { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Snippet { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public bool IsRead { get; set; }
}

public class GetGmailThreadResponseMappingProfile : Profile
{
    public GetGmailThreadResponseMappingProfile()
    {
        CreateMap<Domain.Entities.Message, GetGmailThreadResponse>()
            .ForMember(_ => _.Snippet, opt => opt.MapFrom(src => src.Content));
    }
}