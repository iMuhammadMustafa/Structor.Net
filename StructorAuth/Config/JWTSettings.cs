using Microsoft.Extensions.Configuration;
using Structor.Auth.Entities;

namespace StructorAuth.Config;
public static class JWTSettings
{
    public static string Issuer = default!;
    public static string Audience = default!;
    public static string AccessSecret = default!;
    public static string AccessDuration = default!;
    public static string RefreshSecret = default!;
    public static string RefreshDuration = default!;

    public static void Initialize(JWTOptions jWTOptions)
    {
        
        Issuer = jWTOptions.Issuer ?? throw new NullReferenceException("JWT Issuer is required.");
        Audience = jWTOptions .Audience ?? throw new NullReferenceException("JWT Audience is required.");
        AccessSecret = jWTOptions.AccessSecret ?? throw new NullReferenceException("Access Key is required.");
        AccessDuration = jWTOptions.AccessDuration ?? throw new NullReferenceException("Access Expiry is required.");
        RefreshSecret = jWTOptions.RefreshSecret ?? throw new NullReferenceException("Refresh Key is required.");
        RefreshDuration = jWTOptions.RefreshDuration ?? throw new NullReferenceException("Refresh Expiry is required.");
    }
    internal static void Initialize(IConfiguration configuration)
    {
        //Configuration = configuration;
        Issuer = configuration["JWT:Issuer"] ?? throw new NullReferenceException("JWT Issuer is required.");
        Audience = configuration["JWT:Audience"] ?? throw new NullReferenceException("JWT Audience is required.");
        AccessSecret = configuration["JWT:Keys:Access"] ?? throw new NullReferenceException("Access Key is required.");
        AccessDuration = configuration["JWT:Expiry:Access"] ?? throw new NullReferenceException("Access Expiry is required.");
        RefreshSecret = configuration["JWT:Keys:Refresh"] ?? throw new NullReferenceException("Refresh Key is required.");
        RefreshDuration = configuration["JWT:Expiry:Refresh"] ?? throw new NullReferenceException("Refresh Expiry is required.");
    }

}
