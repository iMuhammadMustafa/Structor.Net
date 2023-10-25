using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Structor.Auth;
using Structor.Auth.Configurations;
using Structor.Core.Configurations;
using Structor.Features.Users;
using Structor.Infrastructure;
using System.Text.Json.Serialization;

namespace Structor.Core;

public static class CoreServicesCollection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultServices();

        services.AddInfrastructureServices(configuration);

        services.AddFeaturesServices(configuration);

        services.AddExternalServices(configuration);

        services.AddAuthenticationServices(configuration);

        return services;
    }
    public static IServiceCollection AddDefaultServices(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });

        services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); //Stringify Enum in JSON
                });

        services.AddSwaggerConfig();

        return services;
    }
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(x => x.FullName);
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            c.AddStructorAuthSwaggerJwtConfig();
        });
        return services;
    }

    public static IServiceCollection AddFeaturesServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddUsersServices(configuration);

        return services;
    }

    public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services;

    }
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var gitHubConfigurations = configuration.GetSection(ConfigurationNames.GITHUB).Get<OAuthOptions>() ?? throw new NullReferenceException("No Configuration for Github available");
        var googleConfigurations = configuration.GetSection(ConfigurationNames.GOOGLE).Get<OAuthOptions>() ?? throw new NullReferenceException("No Configuration for Google available");

        services.AddStructorAuthServices()
                .AddStructorJwtAuth(configuration.GetSection(ConfigurationNames.JWT))
                .AddStructorOAuth()
                .AddGithubOAuth((options) =>
                {
                    options.ClientId = gitHubConfigurations.ClientId;
                    options.ClientSecret = gitHubConfigurations.ClientSecret;
                    options.CallbackUrl = gitHubConfigurations.CallbackUrl;
                    options.DataProtectionSecret = gitHubConfigurations.DataProtectionSecret;
                })
                .AddGoogleOAuth((options) =>
                {
                    options.ClientId = googleConfigurations.ClientId;
                    options.ClientSecret = googleConfigurations.ClientSecret;
                    options.CallbackUrl = googleConfigurations.CallbackUrl;
                    options.DataProtectionSecret = googleConfigurations.DataProtectionSecret;
                });

        return services;
    }
}

/*
Core
    Middleware
    Filters
    
Infrastructure
    DbContext
    Generic Repository
    Notifications
    Generic Exceptions
    Response Model
    Mapper
*/


/*




        //services.AddAuthentication("reddit-cookie")
        //.AddCookie("reddit-cookie", options =>
        //{
        //    options.LoginPath = "/api/Users/LoginExternal";
        //})



        //options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        //    .AddJsonOptions(options =>
        //    {
        //        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        //        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        //    });


       .AddOAuth("reddit", options =>
         {
             options.SignInScheme = "X-Access-Token";


             //Required
             options.ClientId = "223c62ed7f7167523c3a";
             options.ClientSecret = "571fd2e2a9c96612303a7985870b50e9b79e4ca1";


             //options.AuthorizationEndpoint = "https://oauth.mocklab.io/oauth/authorize";
             //options.TokenEndpoint = "https://oauth.mocklab.io/oauth/token";
             //options.UserInformationEndpoint = "https://oauth.mocklab.io/userinfo";





             options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
             options.TokenEndpoint = "https://github.com/login/oauth/access_token";
             options.UserInformationEndpoint = "https://api.github.com/user";


             ////options.ReturnUrlParameter = new Uri("https://localhost:7021/api/Users/SignExternal").ToString();
             //options.ReturnUrlParameter = "/api/Users/SignExternal";
             //Required
             options.CallbackPath = "/api/Users/AfterLogin";

             //Required
             options.Scope.Clear();
             options.Scope.Add("read:user");
             options.SaveTokens = true;


             options.Events.OnCreatingTicket = async context =>
             {
                 using var request = new HttpRequestMessage(HttpMethod.Get, options.UserInformationEndpoint);
                 request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                 using var result = await context.Backchannel.SendAsync(request);

                 var user = await result?.Content?.ReadFromJsonAsync<JsonElement>();

                 context.RunClaimActions(user);


                 var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key-here"));
                 var expiration = DateTime.UtcNow.AddHours(1);

                 // Create claims for the token
                 var claims = new Claim[]
                 {
                    new Claim(ClaimTypes.Name, "John Doe"),
                    new Claim(ClaimTypes.Email, "john.doe@example.com")
                 };

                 // Create the token
                 var token = new JwtSecurityToken(
                     claims: claims,
                     expires: expiration,
                     signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                 );

                 // Generate the token string
                 var tokenString = new JwtSecurityTokenHandler().WriteToken(token);


                 //await HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, principal, authProperties);

             };

         });

        //.AddOpenIdConnect("Reddit", options =>
        //{
        //    options.ClientId = "OcR7QHK62z9AiNZJTxc4_w";
        //    options.ClientSecret = "at__Oz8h3YoLFsaHDVNaJqk_2uwq_A";

        //    options.Authority = "https://www.reddit.com/api/v1/authorize";
        //    options. = "https://www.reddit.com/api/v1/access_token";
        //    options.UserInformationEndpoint = "https://oauth.reddit.com/api/v1/me";


        //    //options.ReturnUrlParameter = new Uri("https://localhost:7021/api/Users/SignExternal").ToString();
        //    options.CallbackPath = "/api/Users/SignExternal";


        //    options.Scope.Clear();
        //    options.Scope.Add("identity");
        //    options.SaveTokens = true;

        //    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        //    //options.ResponseType = "code";
        //    //options.ForwardSignIn = JwtBearerDefaults.AuthenticationScheme;



        //});
        //.AddCookie("reddit-cookie")
        //.AddOAuth("reddit", options =>
        //{
        //    options.SignInScheme = "reddit-cookie";

        //    options.ClientId = "OcR7QHK62z9AiNZJTxc4_w";
        //    options.ClientSecret = "at__Oz8h3YoLFsaHDVNaJqk_2uwq_A";


        //    //options.AuthorizationEndpoint = "https://oauth.mocklab.io/oauth/authorize";
        //    //options.TokenEndpoint = "https://oauth.mocklab.io/oauth/token";
        //    //options.UserInformationEndpoint = "https://oauth.mocklab.io/userinfo";





        //    options.AuthorizationEndpoint = "https://www.reddit.com/api/v1/authorize";
        //    options.TokenEndpoint = "https://www.reddit.com/api/v1/access_token";
        //    options.UserInformationEndpoint = "https://oauth.reddit.com/api/v1/me";


        //    ////options.ReturnUrlParameter = new Uri("https://localhost:7021/api/Users/SignExternal").ToString();
        //    //options.ReturnUrlParameter = "/api/Users/SignExternal";
        //    options.CallbackPath = "/api/Users/SignExternal";


        //    options.Scope.Clear();
        //    options.Scope.Add("identity");
        //    options.SaveTokens = true;

 */
