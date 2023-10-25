using Structor.Auth.DTOs;
using Structor.Features.Users.Dtos;
using System.Net;

namespace Structor.Features.Users.Services
{
    public interface IUserServices
    {
        //Cookie CreateAccessCookie(JwtDto jwt);
        CookieOptions CreateRefreshTokenCookieOptions(JwtDto jwt);
        Task<JwtDto> ValidateAndGenerateNewToken(string userId, string refreshToken);
        string GetProviderRedirect(string provider);
        Task<JwtDto> HandleExternal(string provider, string code, string state);
        Task<JwtDto> Login(LoginDto loginDto);
        Task<JwtDto> Register(NewUserDto newUser);
    }
}