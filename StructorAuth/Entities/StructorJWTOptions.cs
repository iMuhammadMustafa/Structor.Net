namespace StructorAuth.Entities;
public class StructorJWTOptions
{
    public string? AccessCookie { get; set; }
    public string? TokenExpiryHeader { get; set; }

    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience { get; set; } = true;
    public bool ValidateLifetime { get; set; } = true;
    public bool ValidateIssuerSigningKey { get; set; } = true;

    internal bool SetAccessInCookie => !string.IsNullOrWhiteSpace(AccessCookie);
    internal bool SetTokenExpiryHeader => !string.IsNullOrWhiteSpace(TokenExpiryHeader);
}