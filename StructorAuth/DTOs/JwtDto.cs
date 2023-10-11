namespace Structor.Auth.DTOs;

public class JwtDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }

}
