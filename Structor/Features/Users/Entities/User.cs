using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Structor.Infrastructure.Entities;

namespace Structor.Features.Users.Entities;
public class User : IEntity
{
    public string? UserName { get; set; }
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = default!;
    public string? Name { get; set; }

    public string Provider { get; set; } = default!;

    public string? EmailConfirmationCode { get; set; }
    public bool IsEmailConfirmed { get; set; } = false;
    public string? PhoneConfirmationCode { get; set; }
    public bool IsPhoneConfirmed { get; set; } = false;
    public bool IsLocked { get; set; } = false;
    public DateTimeOffset LockoutEndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public bool IsTwoFactorEnabled { get; set; } = false;

    public int AccessFailedCount { get; set; }

}
