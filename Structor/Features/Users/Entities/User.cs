using Structor.Auth.Enums;
using Structor.Infrastructure.Entities;

namespace Structor.Features.Users.Entities;
public class User : IEntity
{
    public string? Username { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PasswordHash { get; set; }
    public string? Name { get; set; }
    public OAuthProvider Provider { get; set; } = OAuthProvider.Local;

    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshExpiry { get; set; }
    //public IEnumerable<string> AccessTokens { get; set; } = new List<string>();
    //public IEnumerable<string> RefreshTokens { get; set; } = new List<string>();
    //public List<string> Sessions { get; set; } = new List<string>();
    public string? EmailConfirmationCode { get; set; }
    public bool IsEmailConfirmed { get; set; } = false;
    public string? PhoneConfirmationCode { get; set; }
    public bool IsPhoneConfirmed { get; set; } = false;
    public bool IsLocked { get; set; } = false;
    public DateTimeOffset? LockoutEndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public bool IsTwoFactorEnabled { get; set; } = false;
    public int AccessFailedCount { get; set; }

}
