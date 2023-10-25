using Structor.Features.Users.Entities;
using Structor.Infrastructure.Repositories;

namespace Structor.Features.Users.Repositories;
public interface IUserRepository : IRepository<User>
{
    Task<bool> UsernameExists(string username);
    Task<bool> EmailExists(string username);
    Task<User?> GetByEmailOrUsername(string loginBy);
}
