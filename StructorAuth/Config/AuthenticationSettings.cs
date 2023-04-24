using Microsoft.Extensions.Configuration;

namespace StructorAuth.Config;
public static class AuthenticationSettings
{
    public static IConfiguration configuration = default!;
    public static string JWT_Issuer = default!;
    public static string JWT_Audience = default!;
    public static string JWT_AccessSecret = default!;
    public static string JWT_AccessDuration = default!;
    public static string JWT_RefreshSecret = default!;
    public static string JWT_RefreshDuration = default!;

    public static void Initialize(IConfiguration Configuration)
    {
        configuration = Configuration;
        JWT_Issuer = configuration["JWT:Issuer"] ?? throw new NullReferenceException("JWT Issuer is required.");
        JWT_Audience = configuration["JWT:Audience"] ?? throw new NullReferenceException("JWT Audience is required.");
        JWT_AccessSecret = configuration["JWT:Keys:Access"] ?? throw new NullReferenceException("Access Key is required.");
        JWT_AccessDuration = configuration["JWT:Expiry:Access"] ?? throw new NullReferenceException("Access Expiry is required.");
        JWT_RefreshSecret = configuration["JWT:Keys:Access"] ?? throw new NullReferenceException("Refresh Key is required.");
        JWT_RefreshDuration = configuration["JWT:Expiry:Access"] ?? throw new NullReferenceException("Refresh Expiry is required.");
    }

}
