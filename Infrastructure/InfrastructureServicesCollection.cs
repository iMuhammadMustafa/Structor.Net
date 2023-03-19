using Structor.Net.Core.DatabaseContext;

namespace Structor.Net.Infrastructure;
public static class FeatureServicesCollection
{

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddDbContext<CoreDbContext>(options => options.UseSqlite(AppSettings._configuration[AppSettings.SqlLiteInMemoryConnection])
                                                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));


        return services;
    }
}
