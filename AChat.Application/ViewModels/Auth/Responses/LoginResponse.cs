using AutoMapper;
using AChat.Domain.Entities;

namespace AChat.Application.ViewModels.Auth.Responses;

public record LoginResponse(string AccessToken, string RefreshToken, UserResponse User, string Role);

public record UserResponse(string Id, string UserName, string Email, string FullName, string AvatarUrl);

public class MapperProfile : Profile
{
	public MapperProfile()
	{
		CreateMap<User, UserResponse>();
	}
}