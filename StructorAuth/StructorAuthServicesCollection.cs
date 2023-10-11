using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Structor.Auth.Entities;
using StructorAuth.Config;
using StructorAuth.Config.Providers;
using StructorAuth.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Structor.Auth;

public static class StructorAuthServicesCollection
{
    private static string _providersPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Config\\Providers";
    public static IServiceCollection AddStructorAuthServices(this IServiceCollection services, Action<AuthOptions>? configureOptions = null)
    {        

        return services;
    }


    public static IServiceCollection AddStructorJwtAuth(this IServiceCollection services, Action<JWTOptions> configureOptions)
    {
        services.AddScoped<IJWTService, JWTService>();

        JWTOptions jWTOptions = new();
        configureOptions(jWTOptions);
        JWTSettings.Initialize(jWTOptions);


        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Events = new JwtBearerEvents();

            //Read AccessToken From Cookie 
            if (jWTOptions.SetAccessInCookie && !string.IsNullOrWhiteSpace(jWTOptions.AccessCookie))
            {
                options.Events.OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies[jWTOptions.AccessCookie];
                    return Task.CompletedTask;
                };
            }
            //Put AccessToken Is Expired In Header
            if (jWTOptions.SetTokenExpiryHeader && !string.IsNullOrWhiteSpace(jWTOptions.TokenExpiryHeader))
            {
                options.Events.OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Append(jWTOptions.TokenExpiryHeader, "true");
                    }
                    return Task.CompletedTask;
                };
            }


            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = jWTOptions.ValidateIssuer,
                ValidateAudience = jWTOptions.ValidateAudience,
                ValidateLifetime = jWTOptions.ValidateLifetime,
                ValidateIssuerSigningKey = jWTOptions.ValidateIssuerSigningKey,
                ValidIssuer = jWTOptions.Issuer,
                ValidAudience = jWTOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWTOptions.AccessSecret ?? ""))
            };
        });


        return services;
    }
    public static void AddStructorAuthSwaggerJwtConfig(this SwaggerGenOptions swaggerGenOptions)
    {
        var securitySchemeName = "Bearer";
        var securityScheme = new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please insert JWT without 'Bearer' Prefix into field",
            Name = "Authorization",
            //Type = SecuritySchemeType.ApiKey,
            Type = SecuritySchemeType.Http, //To remove the need for Bearer Prefix
            BearerFormat = "JWT",
            Scheme = "Bearer"
        };

        swaggerGenOptions.SwaggerGeneratorOptions.SecuritySchemes.Add(securitySchemeName, securityScheme);

        var securityRequirment = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                new string[] { }
            }
        };

        swaggerGenOptions.SwaggerGeneratorOptions.SecurityRequirements.Add(securityRequirment);
    }

    public static IServiceCollection AddStructorOAuth(this IServiceCollection services)
    {
        services.AddScoped<IOAuthService, OAuthService>();

        return services;
    }


    public static IServiceCollection AddGithubOAuth(this IServiceCollection services, Action<OAuthOptions> githubOptions)
    {
        IConfiguration _configuration = new ConfigurationBuilder()
                                            .SetBasePath(_providersPath)
                                            .AddJsonFile("Github.json", optional: false, reloadOnChange: true)
                                            .Build();
        Github.Initialize(_configuration);


        OAuthOptions options = new();
        githubOptions(options);
        Github.Initialize(options);
        return services;
    }
    //public static IServiceCollection AddFacebookOAuth(this IServiceCollection services, Action<OAuthOptions> facebookOptions)
    //{
    //    IConfiguration _configuration = new ConfigurationBuilder()
    //                                        .SetBasePath(_providersPath)
    //                                        .AddJsonFile("Facebook.json", optional: false, reloadOnChange: true)
    //                                        .Build();
    //    Facebook.Initialize(_configuration);


    //    OAuthOptions options = new();
    //    facebookOptions(options);
    //    Facebook.Initialize(options);
    //    return services;
    //}


    /*
        public static AuthenticationBuilder AddGithubJwtOAuth(this AuthenticationBuilder authBuilder)
        {
            authBuilder.AddOAuth("Github", options =>
            {
                options.ForwardAuthenticate = JwtBearerDefaults.AuthenticationScheme;
                options.ClientId = Github.ClientId;
                options.ClientSecret = Github.ClientSecret;


                options.AuthorizationEndpoint = Github.AuthorizationEndpoint;
                options.TokenEndpoint = Github.TokenEndpoint;
                options.UserInformationEndpoint = Github.UserInformationEndpoint;

                options.CallbackPath = "/api/Users/AfterLogin";


                options.Scope.Clear();
                options.Scope.Add(Github.Scope);

                //options.SignInScheme = "X-Access-Token";
                //options.ForwardSignIn = JwtBearerDefaults.AuthenticationScheme;
                //options.SaveTokens = true;

                options.Events.OnCreatingTicket = async context => await OnCreatingTicket(context, options.UserInformationEndpoint);
            });
            return authBuilder;
        }

        public static async Task OnCreatingTicket(OAuthCreatingTicketContext context, string userInformationEndpoint)
        {

            using var request = new HttpRequestMessage(HttpMethod.Get, userInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
            using var result = await context.Backchannel.SendAsync(request);

            var user = await result?.Content?.ReadFromJsonAsync<JsonElement>()!;


            //options.ClaimActions.MapJsonKey(ClaimTypes.Name, "email");
            //context.RunClaimActions(user);

            // Create claims for the token
            var claims = new Dictionary<string, string>();

            claims.Add(ClaimTypes.Name, "John Doe");
            claims.Add(ClaimTypes.Email, "john.doe@example.com");


            // Create the token
            // Generate the token string
            IJWTService jwtService = new JWTService();
            var (accessToken, refreshToken) = jwtService.GenerateJWTokens(claims);


            var responseBody = JsonConvert.SerializeObject(new { accessToken, refreshToken });
            await context.Response.WriteAsync("Hello World");
            context.Response.Redirect("/api/Users/AfterLogin");

            //context.Response.Cookies.Append("X-Access-Token", refreshToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
            //await context.Response.WriteAsync(JsonConvert.SerializeObject(new { accessToken, refreshToken }));

            //await context.Response.Body




            context.Response.Redirect("api/Users/AfterLogin");

            //await HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, principal, authProperties);
        }
    */

        //public static AuthenticationBuilder AddOAuthProvidersConfig(this AuthenticationBuilder authBuilder)
        //{
        //    authBuilder.AddCookie("provider", options =>
        //    {
        //        options.LoginPath = "/login";
        //        options.LogoutPath = "/logout";
        //    });

        //    return authBuilder;
        //}
    }


