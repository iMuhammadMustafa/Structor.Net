using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructorAuth.Config;
using StructorAuth.Services;

namespace StructorAuth;

public static class StructorAuthServicesCollection
{
    public static IServiceCollection AddStructorAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);

        AuthenticationSettings.Initialize(configuration);

        services.AddScoped<IJWTService, JWTService>();


        return services;
    }
}
