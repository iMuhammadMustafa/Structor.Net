using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Structor.Auth.Configurations;
using Structor.Auth.DTOs;

namespace Structor.Auth.Services;

public interface IJWTService
{
    string GenerateJWToken(Dictionary<string, string> _claims);
    JwtDto GenerateJWTokens(Dictionary<string, string> _claims, Dictionary<string, string>? _refreshClaims = null);
    IEnumerable<Claim> ValidateToken(string token, JWTEnum tokenType = JWTEnum.Access);
}
public class JWTService : IJWTService
{
    private readonly ILogger<JWTService> _logger;
    private readonly JwtOptions _jwt;
    private readonly string Issuer;
    private readonly string Audience;
    private readonly string AccessSecret;
    private readonly int AccessDuration;
    private readonly string RefreshSecret;
    private readonly int RefreshDuration;

    public JWTService(IOptions<JwtOptions> jwtOptions, ILogger<JWTService> logger)
    {
        _logger = logger;
        _jwt = jwtOptions.Value;

        if (_jwt == null)
        {
            _logger.LogError("Jwt Isn't correctly instanciated");
            throw new NullReferenceException("Jwt Isn't correctly instanciated");
        }

        Issuer = _jwt.Issuer;
        Audience = _jwt.Audience;
        AccessSecret = _jwt.Keys.Access;
        AccessDuration = _jwt.Durations.Access;
        RefreshSecret = _jwt.Keys.Refresh;
        RefreshDuration = _jwt.Durations.Refresh;


    }

    public string GenerateJWToken(Dictionary<string, string> _claims)
    {
        var accessClaims = GenerateClaims(_claims);
        return GenerateToken(accessClaims, JWTEnum.Access);
    }
    public JwtDto GenerateJWTokens(Dictionary<string, string> _claims, Dictionary<string, string>? _refreshClaims = null)
    {
        var accessClaims = GenerateClaims(_claims);
        var refreshClaims = GenerateClaims(_refreshClaims ?? _claims);


        JwtDto jwt = new()
        {
            AccessToken = GenerateToken(accessClaims, JWTEnum.Access),
            RefreshToken = GenerateToken(refreshClaims, JWTEnum.Refresh)
        };

        return jwt;
    }

    private string GenerateToken(IEnumerable<Claim> claims, JWTEnum keyType)
    {
        string Secret = keyType == JWTEnum.Access ? AccessSecret : RefreshSecret;
        int Duration = keyType == JWTEnum.Access ? AccessDuration : RefreshDuration;

        var expiry = keyType == JWTEnum.Access ? DateTime.UtcNow.AddMinutes(Duration) : DateTime.UtcNow.AddDays(Duration);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(Issuer, Audience, claims, expires: expiry, signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public IEnumerable<Claim> ValidateToken(string token, JWTEnum tokenType = JWTEnum.Access)
    {
        var secret = tokenType == JWTEnum.Access ? AccessSecret : RefreshSecret;
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuer = _jwt.ValidateIssuer,
                    ValidateAudience = _jwt.ValidateAudience,
                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ClockSkew = TimeSpan.Zero
                },
                out SecurityToken validatedToken
            );

            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken.Claims;
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException(ex.Message);
        }
    }

    private IEnumerable<Claim> GenerateClaims(Dictionary<string, string> _claims)
    {
        foreach (var claim in _claims)
        {
            yield return new Claim(claim.Key, claim.Value);
        }
    }
}


