using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Structor.Features.Users;
using Structor.Infrastructure;

namespace Structor.Core;

public static class CoreServicesCollection
{

    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
        //options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        //    .AddJsonOptions(options =>
        //    {
        //        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        //        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        //    });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(x => x.FullName);
        });



        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(x => x.FullName);
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "My API",
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
               {
                 new OpenApiSecurityScheme
                 {
                   Reference = new OpenApiReference
                   {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                   }
                  },
                  new string[] { }
                }
              });
        });

        services.AddAuthentication()
        //    options =>
        //{
        //    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //})
        .AddCookie()
        .AddCookie("reddit-cookie")
        //services.AddAuthentication
        //(options =>
        //{
        //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //})
        //.AddJwtBearer(options =>
        //{
        //    options.Events = new JwtBearerEvents
        //    {
        //        OnMessageReceived = context =>
        //        {
        //            context.Token = context.Request.Cookies["X-Access-Token"];
        //            return Task.CompletedTask;
        //        },

        //        OnAuthenticationFailed = context =>
        //        {
        //            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
        //            {
        //                context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
        //            }
        //            return Task.CompletedTask;
        //        }
        //    };



        //    options.TokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuer = false,
        //        ValidateAudience = false,
        //        ValidateLifetime = false,
        //        ValidateIssuerSigningKey = true,
        //        ValidIssuer = configuration["JWT:Issuer"],
        //        ValidAudience = configuration["JWT:Audience"],
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]))
        //    };

        //})
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
        .AddOAuth("reddit", options =>
         {
             options.SignInScheme = "reddit-cookie";


             //Required
             options.ClientId = "223c62ed7f7167523c3a";
             options.ClientSecret = "571fd2e2a9c96612303a7985870b50e9b79e4ca1";


             //options.AuthorizationEndpoint = "https://oauth.mocklab.io/oauth/authorize";
             //options.TokenEndpoint = "https://oauth.mocklab.io/oauth/token";
             //options.UserInformationEndpoint = "https://oauth.mocklab.io/userinfo";





             options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
             options.TokenEndpoint = "https://www.reddit.com/api/v1/access_token";
             options.UserInformationEndpoint = "https://oauth.reddit.com/api/v1/me";


             ////options.ReturnUrlParameter = new Uri("https://localhost:7021/api/Users/SignExternal").ToString();
             //options.ReturnUrlParameter = "/api/Users/SignExternal";
             //Required
             options.CallbackPath = "/";

             //Required
             options.Scope.Clear();
             options.Scope.Add("read:user");
             options.SaveTokens = true;




         });

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


        services.AddInfrastructureServices(configuration);
        services.AddFeaturesServices(configuration);
        return services;
    }


    public static IServiceCollection AddFeaturesServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddUsersServices(configuration);
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