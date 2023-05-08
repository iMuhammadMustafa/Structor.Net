using Microsoft.Extensions.Configuration;

namespace StructorAuth.Config.Providers;
internal static class Reddit
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
        var redditConfig = AuthenticationSettings.Configuration.GetSection("Reddit");

        var isInDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        if (isInDevelopment && !redditConfig.Exists())
        {
            var providerConfiPath = AuthenticationSettings.Configuration["ProvidersConfigPath"] ?? throw new NullReferenceException("Please add github configuration to user-secrets or json file");
            AuthenticationSettings.Configuration = new ConfigurationBuilder()
                    .SetBasePath(providerConfiPath)
                    .AddJsonFile("Reddit.json", optional: false, reloadOnChange: true)
                    .Build();
            redditConfig = AuthenticationSettings.Configuration.GetSection("Reddit");
        }
        else
        {
            throw new NullReferenceException("Reddit OAuth is not configured.");
        }

        ClientId = redditConfig.GetValue<string>("ClientId") ?? throw new NullReferenceException("ClientId is required.");
        ClientSecret = redditConfig.GetValue<string>("ClientSecret") ?? throw new NullReferenceException("ClientSecret is required.");
        AuthorizationEndpoint = redditConfig.GetValue<string>("AuthorizationEndpoint") ?? throw new NullReferenceException("AuthorizationEndpoint is required.");
        TokenEndpoint = redditConfig.GetValue<string>("TokenEndpoint") ?? throw new NullReferenceException("TokenEndpoint is required.");
        UserInformationEndpoint = redditConfig.GetValue<string>("UserInformationEndpoint") ?? throw new NullReferenceException("UserInformationEndpoint is required.");
        CallbackPath = redditConfig.GetValue<string>("CallbackPath") ?? throw new NullReferenceException("CallbackPath is required.");
        Scope = redditConfig.GetValue<string>("Scope") ?? throw new NullReferenceException("Scope is required.");
    }
}