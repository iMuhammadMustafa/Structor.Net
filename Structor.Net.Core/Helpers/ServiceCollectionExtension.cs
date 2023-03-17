using System.Runtime.CompilerServices;

namespace Structor.Net.Core.Helpers;

public static class ServiceCollectionExtension
{

    public static IServiceCollection AddProgramServices(this IServiceCollection services)
    {

        services.AddCoreServices();
        return services;
    }



    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {

        return services;
    }


}
