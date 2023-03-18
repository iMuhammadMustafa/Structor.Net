using Structor.Net.Infrastructure;

namespace Structor.Net.Core;

public static class CoreServicesCollection
{

    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(x => x.FullName);
        });


        services.AddModulesServices();
        return services;
    }


    public static IServiceCollection AddModulesServices(this IServiceCollection services)
    {
        services.AddInfrastructureServices();

        return services;
    }


}
