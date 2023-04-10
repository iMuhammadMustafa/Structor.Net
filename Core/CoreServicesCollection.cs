using Newtonsoft.Json.Converters;

namespace Core;

public static class CoreServicesCollection
{

    public static IServiceCollection AddCoreServices(this IServiceCollection services)
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


        //services.AddInfrastructureServices();
        //services.AddFeaturessServices();
        return services;
    }


    public static IServiceCollection AddFeaturessServices(this IServiceCollection services)
    {
        return services;
    }


}
