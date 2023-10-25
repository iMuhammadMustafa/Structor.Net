namespace Structor.Auth.Configurations;

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience { get; set; } = true;
    public bool ValidateLifetime { get; set; } = true;
    public bool ValidateIssuerSigningKey { get; set; } = true;
    public Keys Keys { get; set; } = new Keys();
    public Duration Durations { get; set; } = new Duration();
    public CookieHeader CookieHeaders { get; set; } = new CookieHeader();


}
public class Keys
{
    public string Access { get; set; } = string.Empty;
    public  string Refresh { get; set; } = string.Empty;
}
public class Duration
{
    public int Access { get; set; }
    public int Refresh { get; set; } 
}
public class CookieHeader
{
    public string AccessHeader { get; set; } = string.Empty;
    public string RefreshHeader { get; set; } = string.Empty;
    public string AccessExpiryHeader { get; set; } = string.Empty;
    public string RefreshExpiryHeader { get; set; } = string.Empty;
}

public enum JWTEnum
{
    Access,
    Refresh
}