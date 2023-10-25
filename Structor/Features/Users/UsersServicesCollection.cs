using Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Structor.Features.Users.Repositories;
using Structor.Features.Users.Services;

namespace Structor.Features.Users;
public static class UsersServicesCollection
{

    public static IServiceCollection AddUsersServices(this IServiceCollection services, IConfiguration _configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserServices, UserServices>();


        return services;
    }
}
