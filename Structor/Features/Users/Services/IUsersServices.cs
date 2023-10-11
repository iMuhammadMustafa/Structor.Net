using Structor.Auth.DTOs;
using Structor.Auth.Enums;
using Structor.Features.Users.Entities;
using System.Net;
using System.Text.Json.Nodes;

namespace Structor.Features.Users.Services;

public interface IUsersServices
{
    Task<JwtDto> Register(User newUser);
    Task<JwtDto> Login(string username, string password);

    Task<JwtDto> HandleExternal(string provider, string code, string state);
    Cookie CreateAccessCookie(JwtDto jwt);
    Cookie CreateRefreshCookie(JwtDto jwt);

}