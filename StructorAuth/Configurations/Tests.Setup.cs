using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Structor.Auth.Configurations;
using Structor.Auth.Services;

namespace Structor.Auth.Config;
public class DependencySetupFixture : IDisposable
{
    //private readonly string ConfigurationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Config";
    //public IConfigurationRoot Configuration { get; set; }
    public ServiceProvider ServiceProvider { get; private set; }
    public ServiceCollection ServiceCollection { get; private set; }
    public IServiceScope ServiceScope { get; private set; }
    public IServiceProvider ScopeServices { get; private set; }

    public DependencySetupFixture()
    {
        //Configuration = new ConfigurationBuilder()
        //                    .SetBasePath(ConfigurationPath)
        //                    .AddJsonFile("appsettings.json")
        //                    .Build();

        JwtOptions jwt = new()
        {
            Issuer = "Issuer",
            Audience = "Audience",
            Keys = new()
            {
                Access = "SuperSecureTestingKeyVeryVeryVeryLong",
                Refresh = "SuperSecureTestingKeyVeryVeryVeryLong"
            },
            Durations = new()
            {
                Access = 30,
                Refresh = 7
            },
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            CookieHeaders = new()
            {
                AccessHeader = "X-Access",
                AccessExpiryHeader = "X-Access-Expired",
                RefreshHeader = "X-Refresh",
                RefreshExpiryHeader = "X-Refresh-Expired"
            }

        };
         

        ServiceCollection = new ServiceCollection();
        ServiceCollection.AddScoped<IJWTService, JWTService>();
        ServiceCollection.Configure<JwtOptions>((options) =>
        {
            options.Issuer = jwt.Issuer;
            options.Audience = jwt.Audience;

            options.Keys = jwt.Keys;
            options.Durations = jwt.Durations;

            options.CookieHeaders = jwt.CookieHeaders;
        });

        ServiceCollection.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        ServiceProvider = ServiceCollection.BuildServiceProvider();

        ServiceScope = ServiceProvider.CreateScope();
        ScopeServices = ServiceScope.ServiceProvider;
    }

    public void Dispose()
    {
        ServiceScope.Dispose();
    }
}