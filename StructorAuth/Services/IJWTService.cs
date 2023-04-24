using System.Security.Claims;

namespace StructorAuth.Services;
public interface IJWTService
{
    (string, string) GenerateJWTokens(Dictionary<string, string> _claims, Dictionary<string, string>? _refreshClaims = null);
    IEnumerable<Claim> ValidateToken(string token, JWTEnum tokenType = JWTEnum.Access);
}