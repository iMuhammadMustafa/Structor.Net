namespace Structor.Auth.DTOs;
//TODO: Convert this to options object injected from ServicesCollections
public class OAuthResponseEntity
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? ProfileUrl { get; set; }
}
