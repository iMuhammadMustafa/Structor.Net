using Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Structor.Features.UserFeature.Repositories;

namespace Structor.Features.UserFeature;
public static class UsersServicesCollection
{

    public static IServiceCollection AddUsersServices(this IServiceCollection services, IConfiguration _configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();


        return services;
    }
}
