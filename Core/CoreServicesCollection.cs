using Newtonsoft.Json.Converters;
using Structor.Features.UserFeature;
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