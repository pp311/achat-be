using AChat.Application.Common.Interfaces;
using AChat.Application.ViewModels.Auth.Requests;
using AChat.Application.ViewModels.Auth.Responses;
using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Identity;
using AChat.Domain;
using AChat.Domain.Entities;
using AChat.Domain.Exceptions;
using AChat.Domain.Repositories.Base;
using AChat.Domain.Resources;

namespace AChat.Application.Services;

public class AuthService(
    TokenService tokenService,
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUser currentUser) : BaseService(unitOfWork, mapper, currentUser)
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new AuthException();

        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);
        AuthException.ThrowIfFalse(isPasswordValid);

        var token = tokenService.GenerateToken(user);
        var (refreshToken, expires) = tokenService.GenerateRefreshToken();

        await tokenService.SaveRefreshTokenAsync(refreshToken, user.Id, expires);

        var userResponse = Mapper.Map<UserResponse>(user);
        var role = userManager.GetRolesAsync(user).Result.First();

        return new LoginResponse(token, refreshToken, userResponse, role);
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var isEmailTaken = await userManager.FindByEmailAsync(request.Email) != null;
        if (isEmailTaken)
            throw new AlreadyExistsException(nameof(User.Email), request.Email);

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        await UnitOfWork.BeginTransactionAsync();

        try
        {
            var result = await userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
                result = await userManager.AddToRoleAsync(user, AppRole.User.ToValue());

            if (!result.Succeeded)
                throw new AppException(result.Errors.First().Description);

            await UnitOfWork.CommitAsync();
        }
        catch
        {
            await UnitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var refreshTokenEntity = await tokenService.ValidateRefreshTokenAsync(refreshToken);

        if (refreshTokenEntity.IsExpired)
            throw new AppException(ErrorMessages.RefreshTokenExpired);

        var user = await userManager.FindByIdAsync(refreshTokenEntity.UserId.ToString())
                   ?? throw new AuthException();

        var (newRefreshToken, expires) = tokenService.GenerateRefreshToken();

        var tokenDto = new RefreshTokenResponse(tokenService.GenerateToken(user), newRefreshToken);

        refreshTokenEntity.Token = newRefreshToken;
        refreshTokenEntity.Expires = expires;

        await UnitOfWork.SaveChangesAsync();

        return tokenDto;
    }

    public async Task<LoginResponse> GoogleAuthenticateAsync(string accessToken)
    {
        // get user info from access token
        var userInfo = await new Oauth2Service(new BaseClientService.Initializer
        {
            HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken),
            ApplicationName = ""
        }).Userinfo.Get().ExecuteAsync();

        var user = await GetOrCreateUserAsync(userInfo);

        var token = tokenService.GenerateToken(user);
        var (refreshToken, expires) = tokenService.GenerateRefreshToken();

        await tokenService.SaveRefreshTokenAsync(refreshToken, user.Id, expires);

        var userResponse = Mapper.Map<UserResponse>(user);
        // var role = userManager.GetRolesAsync(user).Result.First();
        var role = AppRole.User.ToValue();

        return new LoginResponse(token, refreshToken, userResponse, role);
    }

    private async Task<User> GetOrCreateUserAsync(Userinfo payload)
    {
        var user = await userManager.FindByEmailAsync(payload.Email);
        if (user != null) return user;

        user = new User
        {
            Email = payload.Email,
            UserName = payload.Email,
            FullName = payload.Name,
            EmailConfirmed = true,
            AvatarUrl = payload.Picture
        };

        await UnitOfWork.BeginTransactionAsync();

        try
        {
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
                throw new AppException(result.Errors.First().Description);

            var addRoleResult = await userManager.AddToRoleAsync(user, AppRole.User.ToValue());
            if (!addRoleResult.Succeeded)
                throw new AppException(result.Errors.First().Description);

            await UnitOfWork.SaveChangesAsync();

            await UnitOfWork.CommitAsync();
        }
        catch
        {
            await UnitOfWork.RollbackAsync();
            throw;
        }

        return user;
    }
}
