using Microsoft.Extensions.Configuration;

namespace StructorAuth.Config;
public static class AuthenticationSettings
{
    public static IConfiguration Configuration = default!;
    public static string JWT_Issuer = default!;
    public static string JWT_Audience = default!;
    public static string JWT_AccessSecret = default!;
    public static string JWT_AccessDuration = default!;
    public static string JWT_RefreshSecret = default!;
    public static string JWT_RefreshDuration = default!;

    public static void Initialize(IConfiguration configuration)
    {
        Configuration = configuration;
        JWT_Issuer = Configuration["JWT:Issuer"] ?? throw new NullReferenceException("JWT Issuer is required.");
        JWT_Audience = Configuration["JWT:Audience"] ?? throw new NullReferenceException("JWT Audience is required.");
        JWT_AccessSecret = Configuration["JWT:Keys:Access"] ?? throw new NullReferenceException("Access Key is required.");
        JWT_AccessDuration = Configuration["JWT:Expiry:Access"] ?? throw new NullReferenceException("Access Expiry is required.");
        JWT_RefreshSecret = Configuration["JWT:Keys:Refresh"] ?? throw new NullReferenceException("Refresh Key is required.");
        JWT_RefreshDuration = Configuration["JWT:Expiry:Refresh"] ?? throw new NullReferenceException("Refresh Expiry is required.");
    }

}
