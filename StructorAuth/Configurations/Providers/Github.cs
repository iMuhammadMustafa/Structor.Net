using Microsoft.Extensions.Configuration;
using Structor.Auth.Configurations;
using Structor.Auth.Enums;

namespace Structor.Auth.Config.Providers;
[Obsolete]
internal static class Github
{
    public static string ClientId = null!;
    public static string ClientSecret = null!;
    public static string AuthorizationEndpoint = null!;
    public static string TokenEndpoint = null!;
    public static string UserInformationEndpoint = null!;
    //public static string CallbackBase = null!;
    //public static string CallbackPath = null!;
    //public static string CallbackUrl = null!;
    public static string CallbackUrl = null!;
    public static string RedirectUrl = null!;
    public static string Scope = null!;
    public static string DataProtector = OAuthProvider.Github.ToString();
    public static string DataProtectionSecret = null!;

    public static void Initialize(IConfiguration configuration)
    {
        var githubConfig = configuration.GetSection("Github");

        var isInDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        if (isInDevelopment)
        {
            ClientId = githubConfig.GetValue<string>("ClientId") ?? throw new NullReferenceException("ClientId is required.");
            ClientSecret = githubConfig.GetValue<string>("ClientSecret") ?? throw new NullReferenceException("ClientSecret is required.");
            //RedirectUrl = githubConfig.GetValue<string>("RedirectUrl") ?? throw new NullReferenceException("RedirectUrl is required.");
            CallbackUrl = githubConfig.GetValue<string>("CallbackUrl") ?? throw new NullReferenceException("CallbackUrl is required.");
        }

        AuthorizationEndpoint = githubConfig.GetValue<string>("AuthorizationEndpoint") ?? throw new NullReferenceException("AuthorizationEndpoint is required.");
        TokenEndpoint = githubConfig.GetValue<string>("TokenEndpoint") ?? throw new NullReferenceException("TokenEndpoint is required.");
        UserInformationEndpoint = githubConfig.GetValue<string>("UserInformationEndpoint") ?? throw new NullReferenceException("UserInformationEndpoint is required.");
        Scope = githubConfig.GetValue<string>("Scope") ?? throw new NullReferenceException("Scope is required.");
        DataProtectionSecret = githubConfig.GetValue<string>("DataProtectionSecret") ?? throw new NullReferenceException("DataProtectionSecret is required.");

        //CallbackBase = githubConfig.GetValue<string>("CallbackBase") ?? throw new NullReferenceException("CallbackBase is required.");
        //CallbackPath = githubConfig.GetValue<string>("CallbackPath") ?? throw new NullReferenceException("CallbackPath is required.");

        //CallbackUrl = CallbackBase + CallbackPath;
    }

    public static void Initialize(OAuthOptions oAuthOptions)
    {
        ClientId = oAuthOptions.ClientId ?? throw new NullReferenceException("ClientId is required.");
        ClientSecret = oAuthOptions.ClientSecret ?? throw new NullReferenceException("ClientSecret is required.");
        //RedirectUrl = oAuthOptions.RedirectUrl ?? RedirectUrl ?? throw new NullReferenceException("RedirectUrl is required.");
        //RedirectUrl = RedirectUrl.Replace("provider", OAuthProvidersEnum.Github.ToString());

        CallbackUrl = oAuthOptions.CallbackUrl ?? CallbackUrl ?? throw new NullReferenceException("CallbackUrl is required.");
        CallbackUrl = CallbackUrl.Replace("provider", OAuthProvider.Github.ToString());

        AuthorizationEndpoint = oAuthOptions.AuthorizationEndpoint ?? AuthorizationEndpoint ?? throw new NullReferenceException("AuthorizationEndpoint is required.");
        TokenEndpoint = oAuthOptions.TokenEndpoint ?? TokenEndpoint ?? throw new NullReferenceException("TokenEndpoint is required.");
        UserInformationEndpoint = oAuthOptions.UserInformationEndpoint ?? UserInformationEndpoint ?? throw new NullReferenceException("UserInformationEndpoint is required.");
        Scope = oAuthOptions.Scope ?? Scope ?? throw new NullReferenceException("Scope is required.");
        DataProtectionSecret = oAuthOptions.DataProtectionSecret ?? DataProtectionSecret ?? throw new NullReferenceException("DataProtectionSecret is required.");


        //CallbackBase = oAuthOptions.CallbackBase ?? CallbackBase ?? throw new NullReferenceException("CallbackBase is required.");
        //CallbackPath = oAuthOptions.CallbackPath ?? CallbackPath ?? throw new NullReferenceException("CallbackPath is required.");

        //CallbackUrl = CallbackBase + CallbackPath;
    }







}