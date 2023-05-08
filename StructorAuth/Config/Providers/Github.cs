using Microsoft.Extensions.Configuration;

namespace StructorAuth.Config.Providers;
internal static class Github
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
        var githubConfig = AuthenticationSettings.Configuration.GetSection("Github");

        var isInDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        if (isInDevelopment && !githubConfig.Exists())
        {
            var providerConfiPath = AuthenticationSettings.Configuration["ProvidersConfigPath"] ?? throw new NullReferenceException("Please add github configuration to user-secrets or json file");
            AuthenticationSettings.Configuration = new ConfigurationBuilder()
                    .SetBasePath(providerConfiPath)
                    .AddJsonFile("Github.json", optional: false, reloadOnChange: true)
                    .Build();
            githubConfig = AuthenticationSettings.Configuration.GetSection("Github");
        }
        //else
        //{
        //    throw new NullReferenceException("Github OAuth is not configured");
        //}

        ClientId = githubConfig.GetValue<string>("ClientId") ?? throw new NullReferenceException("ClientId is required.");
        ClientSecret = githubConfig.GetValue<string>("ClientSecret") ?? throw new NullReferenceException("ClientSecret is required.");
        AuthorizationEndpoint = githubConfig.GetValue<string>("AuthorizationEndpoint") ?? throw new NullReferenceException("AuthorizationEndpoint is required.");
        TokenEndpoint = githubConfig.GetValue<string>("TokenEndpoint") ?? throw new NullReferenceException("TokenEndpoint is required.");
        UserInformationEndpoint = githubConfig.GetValue<string>("UserInformationEndpoint") ?? throw new NullReferenceException("UserInformationEndpoint is required.");
        CallbackPath = githubConfig.GetValue<string>("CallbackPath") ?? throw new NullReferenceException("CallbackPath is required.");
        Scope = githubConfig.GetValue<string>("Scope") ?? throw new NullReferenceException("Scope is required.");
    }

}

