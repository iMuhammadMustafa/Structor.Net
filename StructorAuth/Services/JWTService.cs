using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Structor.Auth.DTOs;
using StructorAuth.Config;

namespace StructorAuth.Services;
public enum JWTEnum
{
    Access,
    Refresh
}
public interface IJWTService
{
    JwtDto GenerateJWTokens(Dictionary<string, string> _claims, Dictionary<string, string>? _refreshClaims = null);
    IEnumerable<Claim> ValidateToken(string token, JWTEnum tokenType = JWTEnum.Access);
}
public class JWTService : IJWTService
{
    private readonly string Issuer = JWTSettings.Issuer;
    private readonly string Audience = JWTSettings.Audience;
    private readonly string AccessSecret = JWTSettings.AccessSecret;
    private readonly string AccessDuration = JWTSettings.AccessDuration;
    private readonly string RefreshSecret = JWTSettings.RefreshSecret;
    private readonly string RefreshDuration = JWTSettings.RefreshDuration;


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
        string Duration = keyType == JWTEnum.Access ? AccessDuration : RefreshDuration;

        var expiry = keyType == JWTEnum.Access ? DateTime.UtcNow.AddMinutes(int.Parse(Duration)) : DateTime.UtcNow.AddDays(int.Parse(Duration));
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
                    ValidateIssuer = true,
                    ValidateAudience = true,
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


