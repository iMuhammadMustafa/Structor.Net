using Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;
public static class FeatureServicesCollection
{

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration _configuration)
    {
        services.AddDbContext<CoreDbContext>(options => options.UseSqlite(_configuration["ConnectionStrings:SqlLiteDatabase"])
                                                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));


        return services;
    }
}
