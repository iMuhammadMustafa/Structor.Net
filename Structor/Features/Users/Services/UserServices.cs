using Structor.Auth.DTOs;
using Structor.Features.Users.Entities;
using Structor.Features.Users.Repositories;
using Structor.Auth.Helpers;
using Structor.Auth.Services;
using System.Net;
using Microsoft.Extensions.Options;
using Structor.Auth.Configurations;
using Structor.Auth.Enums;
using System.IdentityModel.Tokens.Jwt;
using Structor.Infrastructure.DTOs.REST;

namespace Structor.Features.Users.Services;

public class UserServices : IUserServices
{
    private readonly IUserRepository _userRepository;
    private readonly IJWTService _jWTService;
    private readonly IOAuthService _oAuthService;
    private readonly ILogger<UserServices> _logger;
    private readonly JwtOptions _Jwtoptions;

    public UserServices(IUserRepository userRepository, IJWTService jWTService,
                        IOAuthService oAuthService,
                        IOptions<JwtOptions> jwtOptions, ILogger<UserServices> logger)
    {
        _userRepository = userRepository;
        _jWTService = jWTService;
        _oAuthService = oAuthService;
        _logger = logger;
        _Jwtoptions = jwtOptions.Value;
    }
    public async Task<Response<JwtDto>> Register(NewUserDto newUser)
    {
        var validateEmailAndUsernameResult = await ValidateEmailAndUsername(newUser.Email, newUser.Username);

        if (validateEmailAndUsernameResult != null)
        {
            return validateEmailAndUsernameResult;
        }


        var hashedPassword = newUser.Password.HashString();
        var refreshToken = AuthenticationUtils.GenerateRandomRefresh();
        var user = new User()
        {
            Email = newUser.Email,
            PasswordHash = hashedPassword,
            Provider = OAuthProvider.Local,
            RefreshToken = refreshToken.HashString(),
            RefreshExpiry = DateTime.UtcNow.AddDays(_Jwtoptions.Durations.Refresh),
        };

        user = await _userRepository.Insert(user, true);

        var jwt = GenerateTokens(user, refreshToken);

        return new Response<JwtDto>().WithData(jwt, StatusCodes.Status201Created);

    }
    public async Task<Response<JwtDto>> Login(string email, string password)
    {
        var dbUser = await _userRepository.GetByEmailOrUsername(email);

        if (dbUser == null)
        {
            return new Response<JwtDto>().WithError("Username or password is incorrect", StatusCodes.Status401Unauthorized);
        }

        var isMatch = password.DoesMatchHash(dbUser.PasswordHash ?? "");

        if (!isMatch)
        {
            return new Response<JwtDto>().WithError("Username or password is incorrect", StatusCodes.Status401Unauthorized);
        }

        var newRefresh = AuthenticationUtils.GenerateRandomRefresh();

        dbUser.RefreshToken = newRefresh.HashString();
        dbUser.RefreshExpiry = DateTime.UtcNow.AddDays(_Jwtoptions.Durations.Refresh);
        dbUser.UpdatedBy = "System";
        dbUser.UpdatedDate = DateTime.UtcNow;

        await _userRepository.Update(dbUser, true);
        return new Response<JwtDto>().WithData(GenerateTokens(dbUser, newRefresh), StatusCodes.Status200OK);
    }
    public async Task<Response<JwtDto>> HandleExternal(string provider, string code, string state)
    {
        /*
         * 1. Get Access Token using code
         * 2. Get User info using Access Token
         * 3. Generate User's claims
         * 4. Generate User's Tokens
         * 5. Add User to database if it doesn't exist & update refresh token
         * 6. Redirect to frontend return URI
         */

        var userData = await _oAuthService.HandleProviderCallback(provider, code, state);
        var providerParsed = Enum.TryParse(provider, out OAuthProvider providerEnum);
        var refreshToken = AuthenticationUtils.GenerateRandomRefresh();

        User newUser = new()
        {
            Email = userData?["email"]?.GetValue<string>() ?? string.Empty,
            Username = userData?["login"]?.GetValue<string>() ?? string.Empty,
            Provider = providerParsed ? providerEnum : OAuthProvider.Local,
            RefreshToken = refreshToken.HashString(),
            RefreshExpiry = DateTime.UtcNow.AddDays(_Jwtoptions.Durations.Refresh)
        };


        var user = await _userRepository.GetByEmailOrUsername(newUser.Email);

        if (user == null)
        {
            user = await _userRepository.Insert(newUser, true);
        }
        else
        {
            user.RefreshToken = refreshToken.HashString();
            user.RefreshExpiry = DateTime.UtcNow.AddDays(_Jwtoptions.Durations.Refresh);
            await _userRepository.Update(user, true);
        }

        var jwt = GenerateTokens(user, refreshToken);
        return new Response<JwtDto>().WithData(jwt, StatusCodes.Status201Created);
    }
    public Cookie CreateRefreshCookie(JwtDto jwt)
    {
        var cookie = new Cookie(_Jwtoptions.CookieHeaders.RefreshHeader, jwt.RefreshToken)
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.Now.AddDays(_Jwtoptions.Durations.Refresh),
        };

        return cookie;
    }
    public Cookie CreateAccessCookie(JwtDto jwt)
    {
        var address = Dns.GetHostAddresses(Dns.GetHostName())
                 .FirstOrDefault(addr => !IPAddress.IsLoopback(addr));
        var cookie = new Cookie(_Jwtoptions.CookieHeaders.AccessHeader, jwt.AccessToken)
        {
            Path = address?.ToString() ?? "localhost",
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.Now.AddDays(_Jwtoptions.Durations.Access),
        };

        return cookie;
    }

    public string GetProviderRedirect(string provider)
    {
        return _oAuthService.GetProviderRedirect(provider);
    }

    private async Task<Response<JwtDto>?> CheckEmailExistance(string email)
    {
        var doesEmailExists = await _userRepository.DoesEmailExists(email);
        if (!doesEmailExists)
        {
            return new Response<JwtDto>().WithError("Email already exists", StatusCodes.Status409Conflict);
        }

        return null;
    }
    private async Task<Response<JwtDto>?> CheckUsernameExistance(string username)
    {
        var doesEmailExists = await _userRepository.DoesUsernameExist(username);
        if (!doesEmailExists)
        {
            return new Response<JwtDto>().WithError("Username already exists", StatusCodes.Status409Conflict);
        }

        return null;
    }
    private async Task<Response<JwtDto>?> ValidateEmailAndUsername(string email, string? username = null)
    {
        var emailExists = await CheckEmailExistance(email);
        if (emailExists != null)
        {
            return emailExists;
        }

        if (!string.IsNullOrWhiteSpace(username))
        {
            var usernameExists = await CheckUsernameExistance(username);
            if (emailExists != null)
            {
                return emailExists;
            }
        }

        return null;
    }
    private JwtDto GenerateTokens(User user, string? refreshToken = null)
    {
        var claims = GenerateClaims(user);
        JwtDto jwt = new()
        {
            AccessToken = _jWTService.GenerateJWToken(claims),
            RefreshToken = refreshToken ?? AuthenticationUtils.GenerateRandomRefresh()
        };


        return jwt;
    }
    private Dictionary<string, string> GenerateClaims(User user)
    {
        var claims = new Dictionary<string, string>();
        claims.Add(JwtRegisteredClaimNames.Sub, user.Guid.ToString());
        claims.Add(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
        claims.Add(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString());
        claims.Add("Email", user.Email);
        claims.Add("Name", user.Name ?? "");
        return claims;
    }

}
