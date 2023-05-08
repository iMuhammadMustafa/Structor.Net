using Microsoft.Extensions.Configuration;

namespace StructorAuth.Config.Providers;
internal static class Facebook
{
    public static string ClientId = null!;
    public static string ClientSecret = null!;
    public static string AuthorizationEndpoint = null!;
    public static string TokenEndpoint = null!;
    public static string UserInformationEndpoint = null!;
    public static string CallbackPath = null!;
    public static string Scope = null!;

    public static void Initialize()
    {
        var facebookConfig = AuthenticationSettings.Configuration.GetSection("Facebook");

        var isInDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        if (isInDevelopment && !facebookConfig.Exists())
        {
            var providerConfiPath = AuthenticationSettings.Configuration["ProvidersConfigPath"] ?? throw new NullReferenceException("Please add github configuration to user-secrets or json file");
            AuthenticationSettings.Configuration = new ConfigurationBuilder()
                    .SetBasePath(providerConfiPath)
                    .AddJsonFile("Facebook.json", optional: false, reloadOnChange: true)
                    .Build();
            facebookConfig = AuthenticationSettings.Configuration.GetSection("Facebook");
        }
        else
        {
            throw new NullReferenceException("Facebook OAuth is not configured");
        }


        ClientId = facebookConfig.GetValue<string>("ClientId") ?? throw new NullReferenceException("ClientId is required.");
        ClientSecret = facebookConfig.GetValue<string>("ClientSecret") ?? throw new NullReferenceException("ClientSecret is required.");
        AuthorizationEndpoint = facebookConfig.GetValue<string>("AuthorizationEndpoint") ?? throw new NullReferenceException("AuthorizationEndpoint is required.");
        TokenEndpoint = facebookConfig.GetValue<string>("TokenEndpoint") ?? throw new NullReferenceException("TokenEndpoint is required.");
        UserInformationEndpoint = facebookConfig.GetValue<string>("UserInformationEndpoint") ?? throw new NullReferenceException("UserInformationEndpoint is required.");
        CallbackPath = facebookConfig.GetValue<string>("CallbackPath") ?? throw new NullReferenceException("CallbackPath is required.");
        Scope = facebookConfig.GetValue<string>("Scope") ?? throw new NullReferenceException("Scope is required.");
    }
}