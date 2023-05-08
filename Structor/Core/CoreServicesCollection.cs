using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Structor.Features.Users;
using Structor.Infrastructure;
using StructorAuth;

namespace Structor.Core;

public static class CoreServicesCollection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter()); //Stringify Enum in JSON
                });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerConfig();

        services.AddAuthenticationConfig(configuration);
        services.AddFeaturesServices(configuration);

        return services;
    }

    public static IServiceCollection AddFeaturesServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureServices(configuration);
        services.AddUsersServices(configuration);


        return services;
    }


    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(x => x.FullName);
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            c.AddStructorSwaggerJwtConfig();
        });
        return services;
    }

    public static IServiceCollection AddAuthenticationConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStructorAuthServices(configuration);
        services.AddStructorJwtAuthentication((options) =>
        {
            options.TokenExpiryHeader = "IsTokenExpired";
        });
        //.AddOAuthProvidersConfig()
        //.AddGithubJwtOAuth();



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




        //});

        //.AddMicrosoftAccount(options =>
        //{
        //    options.ClientId = "0cd12960-1d7d-4fb8-8e05-73c32e370d2d";
        //    options.ClientSecret = "v4r8Q~IGn7LKWelFMQ~Dl1GjpKGVm8Iq13uFTdeD";

        //});
        //.AddGoogle(options =>
        //{
        //    options.ClientId = "your-client-idzzzzzzz";
        //    options.ClientSecret = "your-client-idzzzzzzz";
        //});
        //.AddOpenIdConnect(options =>
        //{
        //    options.Authority = "localhost";
        //    options.ClientId = "your-client-idzzzzzzz";
        //    options.ClientSecret = "your-client-secretzzzzzzz";
        //    options.ResponseType = "code";
        //    options.Scope.Add("openid");
        //    options.Scope.Add("profile");
        //    options.SaveTokens = true;
        //});
 */
