using Structor.Auth.Enums;
using Structor.Infrastructure.Entities;

namespace Structor.Features.Users;

public class NewUserDto : IEntity
{
    public string? Username { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string Password { get; set; }
    public string? Name { get; set; }
    public OAuthProvider Provider { get; set; } = OAuthProvider.Local;
}
