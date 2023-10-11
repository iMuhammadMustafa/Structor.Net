namespace Structor.Auth.Entities;
public class JWTOptions
{

    public string? Issuer { get; set; }
    public string? Audience {get; set;}
    public string? AccessSecret {get; set;}
    public string? AccessDuration {get; set;}
    public string? RefreshSecret {get; set;}
    public string? RefreshDuration {get; set;}
    public string? AccessCookie { get; set; }
    public string? TokenExpiryHeader { get; set; }

    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience { get; set; } = true;
    public bool ValidateLifetime { get; set; } = true;
    public bool ValidateIssuerSigningKey { get; set; } = true;

    internal bool SetAccessInCookie => !string.IsNullOrWhiteSpace(AccessCookie);
    internal bool SetTokenExpiryHeader => !string.IsNullOrWhiteSpace(TokenExpiryHeader);
}