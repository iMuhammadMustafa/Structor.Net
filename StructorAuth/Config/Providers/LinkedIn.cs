using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace StructorAuth.Config.Providers;
internal static class LinkedIn
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
        var linkedinConfig = AuthenticationSettings.Configuration.GetSection("LinkedIn");

        var isInDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        if (isInDevelopment && !linkedinConfig.Exists())
        {
            var providerConfiPath = AuthenticationSettings.Configuration["ProvidersConfigPath"] ?? throw new NullReferenceException("Please add github configuration to user-secrets or json file");
            AuthenticationSettings.Configuration = new ConfigurationBuilder()
                    .SetBasePath(providerConfiPath)
                    .AddJsonFile("LinkedIn.json", optional: false, reloadOnChange: true)
                    .Build();
            linkedinConfig = AuthenticationSettings.Configuration.GetSection("LinkedIn");
        }
        else
        {
            throw new NullReferenceException("LinkedIn OAuth is not configured");
        }


        ClientId = linkedinConfig.GetValue<string>("ClientId") ?? throw new NullReferenceException("ClientId is required.");
        ClientSecret = linkedinConfig.GetValue<string>("ClientSecret") ?? throw new NullReferenceException("ClientSecret is required.");
        AuthorizationEndpoint = linkedinConfig.GetValue<string>("AuthorizationEndpoint") ?? throw new NullReferenceException("AuthorizationEndpoint is required.");
        TokenEndpoint = linkedinConfig.GetValue<string>("TokenEndpoint") ?? throw new NullReferenceException("TokenEndpoint is required.");
        UserInformationEndpoint = linkedinConfig.GetValue<string>("UserInformationEndpoint") ?? throw new NullReferenceException("UserInformationEndpoint is required.");
        CallbackPath = linkedinConfig.GetValue<string>("CallbackPath") ?? throw new NullReferenceException("CallbackPath is required.");
        Scope = linkedinConfig.GetValue<string>("Scope") ?? throw new NullReferenceException("Scope is required.");
    }
}