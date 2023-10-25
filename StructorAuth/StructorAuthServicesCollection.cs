using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Structor.Auth.Configurations;
using Structor.Auth.Enums;
using Structor.Auth.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Structor.Auth;

public static class StructorAuthServicesCollection
{
    private static string _providersPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Configurations";

    private static readonly IConfiguration _configuration = new ConfigurationBuilder()
                                            .SetBasePath(_providersPath)
                                            .AddJsonFile("Providers.json", optional: false, reloadOnChange: true)
                                            .Build();
    public static IServiceCollection AddStructorAuthServices(this IServiceCollection services, Action<AuthOptions>? configureOptions = null)
    {        

        return services;
    }

    #region Jwt
    public static IServiceCollection AddStructorJwtAuth(this IServiceCollection services, Action<JwtOptions> jwtAction )
    {
        services.Configure(jwtAction);

        JwtOptions jwtOptions = new();
        jwtAction(jwtOptions);

        services.AddStructorJwtAuth(jwtOptions);

        return services;
    }
    public static IServiceCollection AddStructorJwtAuth(this IServiceCollection services, IConfiguration jwtOptionsSection)
    {
        JwtOptions jwtOptions = jwtOptionsSection.Get<JwtOptions>() ?? throw new NullReferenceException("Jwt Configuration Settings is not available, please add it.");

        services.Configure<JwtOptions>(jwtOptionsSection);

        services.AddStructorJwtAuth(jwtOptions);

        return services;
    }
    private static IServiceCollection AddStructorJwtAuth(this IServiceCollection services, JwtOptions jwtOptions)
    {
        services.AddScoped<IJWTService, JWTService>();

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
            if (!string.IsNullOrWhiteSpace(jwtOptions.CookieHeaders?.AccessHeader))
            {
                options.Events.OnMessageReceived = (context) =>
                {
                    if (!string.IsNullOrWhiteSpace(context.Request.Cookies[jwtOptions.CookieHeaders.AccessHeader]))
                    {
                        context.Token = context.Request.Cookies[jwtOptions.CookieHeaders.AccessHeader];
                    }
                    return Task.CompletedTask;
                };
            }
            //Put AccessToken Is Expired In Header
            if (!string.IsNullOrWhiteSpace(jwtOptions.CookieHeaders?.AccessExpiryHeader))
            {
                options.Events.OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Append(jwtOptions.CookieHeaders.AccessExpiryHeader, "true");
                    }
                    return Task.CompletedTask;
                };
            }

            if (jwtOptions.Keys == null)
            {
                throw new NullReferenceException("Jwt Keys are required to be set.");
            }

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = jwtOptions.ValidateIssuer,
                ValidateAudience = jwtOptions.ValidateAudience,
                ValidateLifetime = jwtOptions.ValidateLifetime,
                ValidateIssuerSigningKey = jwtOptions.ValidateIssuerSigningKey,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Keys.Access))
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
    #endregion

    #region OAuth
    public static IServiceCollection AddStructorOAuth(this IServiceCollection services)
    {
        services.AddScoped<IOAuthService, OAuthService>();

        return services;
    }
    public static IServiceCollection AddGithubOAuth(this IServiceCollection services, Action<OAuthOptions> optionsActions)
    {
        services.AddOAuthProvider(OAuthProvider.Github, optionsActions);
        return services;
    }
    public static IServiceCollection AddGoogleOAuth(this IServiceCollection services, Action<OAuthOptions> optionsActions)
    {
        services.AddOAuthProvider(OAuthProvider.Google, optionsActions);
        return services;
    }

    private static IServiceCollection AddOAuthProvider(this IServiceCollection services, OAuthProvider oAuthProvider, Action<OAuthOptions> optionsActions)
    {
        var provider = oAuthProvider.ToString();
        var options = GetOAuthConfiguration(oAuthProvider, _configuration.GetSection(provider));

        optionsActions(options);

        services.Configure<OAuthOptions>(provider, obj =>
        {
            obj.ClientId = options.ClientId;
            obj.ClientSecret = options.ClientSecret;
            obj.RedirectUrl = options.RedirectUrl;
            obj.AuthorizationEndpoint = options.AuthorizationEndpoint;
            obj.TokenEndpoint = options.TokenEndpoint;
            obj.UserInformationEndpoint = options.UserInformationEndpoint;
            obj.Scope = options.Scope;
            obj.CallbackUrl = options.CallbackUrl;
            obj.DataProtector = options.DataProtector;
            obj.DataProtectionSecret = options.DataProtectionSecret;
            obj.OAuthProvider = options.OAuthProvider;
        });

        //services.AddSingleton<IOptionsMonitor<OAuthOptions>>(IOptionsFactory<OAuthOptions>);

        return services;
    }

    private static OAuthOptions GetOAuthConfiguration(OAuthProvider oAuthProvider, IConfigurationSection configurationSection)
    {
        OAuthOptions options = new()
        {
            OAuthProvider = oAuthProvider,
            AuthorizationEndpoint = configurationSection.GetValue<string>("AuthorizationEndpoint") ?? throw new NullReferenceException("AuthorizationEndpoint is required."),
            TokenEndpoint = configurationSection.GetValue<string>("TokenEndpoint") ?? throw new NullReferenceException("TokenEndpoint is required."),
            UserInformationEndpoint = configurationSection.GetValue<string>("UserInformationEndpoint") ?? throw new NullReferenceException("UserInformationEndpoint is required."),
            Scope = configurationSection.GetValue<string>("Scope") ?? throw new NullReferenceException("Scope is required."),
        };

        return options;
    }

    #endregion
    //public static IServiceCollection AddStructorJwtAuth(this IServiceCollection services, Action<JWTOptions> configureOptions)
    //{
    //    services.AddScoped<IJWTService, JWTService>();

    //    JWTOptions jWTOptions = new();
    //    configureOptions(jWTOptions);
    //    JWTSettings.Initialize(jWTOptions);


    //    services.AddAuthentication(options =>
    //    {
    //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    //    })
    //    .AddJwtBearer(options =>
    //    {
    //        options.Events = new JwtBearerEvents();

    //        //Read AccessToken From Cookie 
    //        if (jWTOptions.SetAccessInCookie && !string.IsNullOrWhiteSpace(jWTOptions.AccessCookie))
    //        {
    //            options.Events.OnMessageReceived = context =>
    //            {
    //                context.Token = context.Request.Cookies[jWTOptions.AccessCookie];
    //                return Task.CompletedTask;
    //            };
    //        }
    //        //Put AccessToken Is Expired In Header
    //        if (jWTOptions.SetTokenExpiryHeader && !string.IsNullOrWhiteSpace(jWTOptions.TokenExpiryHeader))
    //        {
    //            options.Events.OnAuthenticationFailed = context =>
    //            {
    //                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
    //                {
    //                    context.Response.Headers.Append(jWTOptions.TokenExpiryHeader, "true");
    //                }
    //                return Task.CompletedTask;
    //            };
    //        }


    //        options.TokenValidationParameters = new TokenValidationParameters
    //        {
    //            ValidateIssuer = jWTOptions.ValidateIssuer,
    //            ValidateAudience = jWTOptions.ValidateAudience,
    //            ValidateLifetime = jWTOptions.ValidateLifetime,
    //            ValidateIssuerSigningKey = jWTOptions.ValidateIssuerSigningKey,
    //            ValidIssuer = jWTOptions.Issuer,
    //            ValidAudience = jWTOptions.Audience,
    //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWTOptions.AccessSecret ?? ""))
    //        };
    //    });


    //    return services;
    //}
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


