using Structor.Auth.DTOs;
using Structor.Features.Users.Entities;
using Structor.Features.Users.Repositories;
using Structor.Auth.Helpers;
using Structor.Auth.Services;
using Microsoft.Extensions.Options;
using Structor.Auth.Configurations;
using Structor.Auth.Enums;
using Structor.Core.Exceptions;
using Structor.Features.Users.Dtos;
using Microsoft.IdentityModel.JsonWebTokens;
using Structor.Features.Enums;

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
    public async Task<JwtDto> Register(NewUserDto newUser)
    {
        await CheckAndThrowIfUserExists(newUser);


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

        return jwt;

    }
    public async Task<JwtDto> Login(LoginDto loginDto)
    {
        User dbUser = await ValidateUserCredentials(loginDto);

        var newRefresh = AuthenticationUtils.GenerateRandomRefresh();

        UpdateUserRefreshToken(dbUser, newRefresh);

        await _userRepository.Update(dbUser, true);
        return GenerateTokens(dbUser, newRefresh);
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

        var userData = await _oAuthService.HandleProviderCallback(provider, code, state) ?? throw new HttpException("Failed to verify OAuth", StatusCodes.Status401Unauthorized);
        var providerParsed = _oAuthService.GetProviderEnum(provider);
        var refreshToken = AuthenticationUtils.GenerateRandomRefresh();

        User newUser = new()
        {
            Email = userData?["email"]?.GetValue<string>() ?? string.Empty,
            Username = userData?["login"]?.GetValue<string>() ?? string.Empty,
            Provider = providerParsed,
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
            UpdateUserRefreshToken(user, refreshToken);
            await _userRepository.Update(user, true);
        }

        var jwt = GenerateTokens(user, refreshToken);
        return jwt;
    }

    public async Task<JwtDto> ValidateAndGenerateNewToken(string userId, string refreshToken)
    {
        var user = await _userRepository.GetByGuid(Guid.Parse(userId));

        if (user == null)
        {
            throw new UnauthorizedAccessException("User does not exist");
        }
        if (string.IsNullOrWhiteSpace(user.RefreshToken) || !refreshToken.DoesMatchHash(user.RefreshToken) || user.RefreshExpiry <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh Token is not valid");
        }

        var newRefreshToken = AuthenticationUtils.GenerateRandomRefresh();

        UpdateUserRefreshToken(user, newRefreshToken);
        await _userRepository.Update(user, true);

        return GenerateTokens(user, refreshToken);
    }

    public CookieOptions CreateRefreshTokenCookieOptions(JwtDto jwt)
    {
        var cookie = new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.Now.AddDays(_Jwtoptions.Durations.Refresh),
        };
        return cookie;
    }
    //public Cookie CreateAccessCookie(JwtDto jwt)
    //{
    //    var address = Dns.GetHostAddresses(Dns.GetHostName())
    //             .FirstOrDefault(addr => !IPAddress.IsLoopback(addr));
    //    var cookie = new Cookie(_Jwtoptions.CookieHeaders.AccessHeader, jwt.AccessToken)
    //    {
    //        Path = address?.ToString() ?? "localhost",
    //        HttpOnly = true,
    //        Secure = true,
    //        Expires = DateTime.Now.AddDays(_Jwtoptions.Durations.Access),
    //    };

    //    return cookie;
    //}
    public string GetProviderRedirect(string provider)
    {
        return _oAuthService.GetProviderRedirect(provider);
    }
    private async Task CheckAndThrowIfUserExists(NewUserDto newUser)
    {
        await ThrowIfEmailExists(newUser.Email);

        if (!string.IsNullOrWhiteSpace(newUser.Username))
        {
            await ThrowIfUsernameExists(newUser.Username);
        }
    }
    private async Task ThrowIfEmailExists(string email)
    {
        var emailExists = await _userRepository.EmailExists(email);
        if (emailExists)
        {
            throw new HttpException("Email already exists", StatusCodes.Status409Conflict);
        }
    }
    private async Task ThrowIfUsernameExists(string username)
    {
        var usernameExists = await _userRepository.UsernameExists(username);
        if (usernameExists)
        {
            throw new HttpException("Username already exists", StatusCodes.Status409Conflict);
        }

    }
    private async Task<User> ValidateUserCredentials(LoginDto loginDto)
    {
        var dbUser = await _userRepository.GetByEmailOrUsername(loginDto.UsernameOrEmail);

        if (dbUser == null)
        {
            throw new HttpException("Username or password is incorrect", StatusCodes.Status401Unauthorized);
        }

        var isMatch = loginDto.Password.DoesMatchHash(dbUser.PasswordHash ?? "");

        if (!isMatch)
        {
            throw new HttpException("Username or password is incorrect", StatusCodes.Status401Unauthorized);
        }

        return dbUser;
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
        //claims.Add(JwtRegisteredClaimNames.Sub, user.Guid.ToString());
        //claims.Add(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
        //claims.Add(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString());
        claims.Add(UserClaim.Id, user.Id.ToString());
        claims.Add(UserClaim.Guid, user.Guid.ToString());
        claims.Add(UserClaim.Name, user.Name ?? "");
        claims.Add(UserClaim.Username, user.Username ?? "");
        claims.Add(UserClaim.Email, user.Email);
        return claims;
    }
    private void UpdateUserRefreshToken(User user, string newRefresh)
    {
        user.RefreshToken = newRefresh.HashString();
        user.RefreshExpiry = DateTime.UtcNow.AddDays(_Jwtoptions.Durations.Refresh);
        user.UpdatedBy = "System";
        user.UpdatedDate = DateTime.UtcNow;
    }
}
