using Microsoft.Extensions.Configuration;

namespace StructorAuth.Config.Providers;
internal static class Twitter
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
        var twitterConfig = AuthenticationSettings.Configuration.GetSection("Twitter");


        var isInDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        if (isInDevelopment && !twitterConfig.Exists())
        {
            var providerConfiPath = AuthenticationSettings.Configuration["ProvidersConfigPath"] ?? throw new NullReferenceException("Please add twitter configuration to user-secrets or json file");
            AuthenticationSettings.Configuration = new ConfigurationBuilder()
                    .SetBasePath(providerConfiPath)
                    .AddJsonFile("Twitter.json", optional: false, reloadOnChange: true)
                    .Build();
            twitterConfig = AuthenticationSettings.Configuration.GetSection("Twitter");
        }
        else
        {
            throw new NullReferenceException("Twitter OAuth is not configured");
        }

        ClientId = twitterConfig.GetValue<string>("ClientId") ?? throw new NullReferenceException(nameof(ClientId));
        ClientSecret = twitterConfig.GetValue<string>("ClientSecret") ?? throw new NullReferenceException(nameof(ClientSecret));
        AuthorizationEndpoint = twitterConfig.GetValue<string>("AuthorizationEndpoint") ?? throw new NullReferenceException(nameof(AuthorizationEndpoint));
        TokenEndpoint = twitterConfig.GetValue<string>("TokenEndpoint") ?? throw new NullReferenceException(nameof(TokenEndpoint));
        UserInformationEndpoint = twitterConfig.GetValue<string>("UserInformationEndpoint") ?? throw new NullReferenceException(nameof(UserInformationEndpoint));
        CallbackPath = twitterConfig.GetValue<string>("CallbackPath") ?? throw new NullReferenceException(nameof(CallbackPath));
        Scope = twitterConfig.GetValue<string>("Scope") ?? throw new NullReferenceException(nameof(Scope));
    }
}
