using Structor.Auth.DTOs;
using Structor.Features.Users.Entities;
using Structor.Features.Users.Repositories;
using Structor.Auth.Config;
using Structor.Auth.Helpers;
using Structor.Auth.Services;
using System.Net;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using Structor.Auth.Configurations;

namespace Structor.Features.Users.Services;

public class UsersServices : IUsersServices
{
    private readonly IUserRepository _userRepository;
    private readonly IJWTService _jWTService;
    private readonly IOAuthService _oAuthService;
    private readonly IOptions<JwtOptions> _options;

    public UsersServices(IUserRepository userRepository, IJWTService jWTService, IOAuthService oAuthService, IOptions<JwtOptions> options)
    {
        _userRepository = userRepository;
        _jWTService = jWTService;
        _oAuthService = oAuthService;
        _options = options;
    }



    public async Task<JwtDto> Register(User newUser)
    {
        /*
         * 1. Generate Claims
         * 2. Generate Access and Refresh Tokens
         * 3. Insert to database
         * 4. return response
         */
        newUser.Password = AuthenticationUtils.HashString(newUser.Password);
        var user = await _userRepository.Insert(newUser, true);


        return GenerateTokens();

    }

    public async Task<JwtDto> Login(string username, string password)
    {
        /*
         * 1. Get User from database
         * 2. Verify User exists and Password is correct
         * 3. Generate claims and tokens
         * 4. return Tokens
         */
        var dbUser = await _userRepository.FindFirst(x => x.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));

        var isMatch = password.DoesMatchHash(dbUser.Password);

        if (isMatch)
        {
            return GenerateTokens();
        }

        throw new UnauthorizedAccessException();

    }

    private JwtDto GenerateTokens()
    {
        var claims = GenerateClaims();
        var jwt = _jWTService.GenerateJWTokens(claims);

        return jwt;
    }

    private Dictionary<string, string> GenerateClaims()
    {
        var claims = new Dictionary<string, string>();
        claims.Add("email", "id");
        claims.Add("Name", "name");

        return claims;
    }


    public async Task<JwtDto> HandleExternal(string provider, string code, string state)
    {

        /*
         * 1. Get Access Token using code
         * 2. Get User info using Access Token
         * 3. Generate User's claims
         * 4. Generate User's Tokens
         * 5. Add User to database if it doesn't exist & update refresh token
         * 6. Redirect to frontend return URI
         */

        //var obj = JsonSerializer.Deserialize<dynamic>(content);

        var userData = await _oAuthService.HandleProviderCallback(provider, code, state);



        var claims = new Dictionary<string, string>
        {
            { "Email", userData?["email"]?.GetValue<string>() ?? string.Empty },
            { "Username", userData?["login"]?.GetValue<string>() ?? string.Empty },
            { "Profile", userData?["url"] ?.GetValue < string >() ?? string.Empty },
            { "Provider", provider }
        };



        var jwt = _jWTService.GenerateJWTokens(claims);
        return jwt;
    }

    public Cookie CreateRefreshCookie(JwtDto jwt)
    {
        var cookie = new Cookie("X-RefreshToken", jwt.RefreshToken)
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.Now.AddDays(double.Parse(_options.Value.Durations.Refresh)),
        };

        return cookie;
    }
    public Cookie CreateAccessCookie(JwtDto jwt)
    {
        var cookie = new Cookie("X-AccessToken", jwt.AccessToken)
        {
            Path = "https://localhost:5001",
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.Now.AddDays(double.Parse(_options.Value.Durations.Access)),
        };

        return cookie;
    }

}
