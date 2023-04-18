using Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Structor.Infrastructure;
public static class FeatureServicesCollection
{

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration _configuration)
    {

        services.AddDbContext<CoreDbContext>(options => options.UseSqlite(_configuration["ConnectionStrings:SqlLiteDatabase"])
                                                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        return services;
    }
}
