using Structor.Auth.DTOs;
using Structor.Infrastructure.DTOs.REST;
using System.Net;

namespace Structor.Features.Users.Services
{
    public interface IUserServices
    {
        Cookie CreateAccessCookie(JwtDto jwt);
        Cookie CreateRefreshCookie(JwtDto jwt);
        string GetProviderRedirect(string provider);
        Task<Response<JwtDto>> HandleExternal(string provider, string code, string state);
        Task<Response<JwtDto>> Login(string email, string password);
        Task<Response<JwtDto>> Register(NewUserDto newUser);
    }
}