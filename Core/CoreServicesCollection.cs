using Microsoft.EntityFrameworkCore;
using Structor.Net.Core.DatabaseContext;
using Structor.Net.Core.Globals;
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


        services.AddInfrastructureServices();
        services.AddFeaturessServices();
        return services;
    }


    public static IServiceCollection AddFeaturessServices(this IServiceCollection services)
    {
        return services;
    }


}
