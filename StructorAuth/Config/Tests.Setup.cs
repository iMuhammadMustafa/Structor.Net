using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructorAuth.Services;

namespace StructorAuth.Config;
public class DependencySetupFixture : IDisposable
{
    private readonly string ConfigurationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Config";
    public IConfigurationRoot Configuration { get; set; }
    public ServiceProvider ServiceProvider { get; private set; }
    public ServiceCollection ServiceCollection { get; private set; }
    public IServiceScope ServiceScope { get; private set; }
    public IServiceProvider ScopeServices { get; private set; }

    public DependencySetupFixture()
    {
        Configuration = new ConfigurationBuilder()
                            .SetBasePath(ConfigurationPath)
                            .AddJsonFile("appsettings.json")
                            .Build();


        ServiceCollection = new ServiceCollection();
        ServiceCollection.AddScoped<IJWTService, JWTService>();
        ServiceCollection.AddSingleton<IConfiguration>(Configuration);
        JWTSettings.Initialize(Configuration);
        ServiceProvider = ServiceCollection.BuildServiceProvider();

        ServiceScope = ServiceProvider.CreateScope();
        ScopeServices = ServiceScope.ServiceProvider;
    }

    public void Dispose()
    {
        ServiceScope.Dispose();
    }
}