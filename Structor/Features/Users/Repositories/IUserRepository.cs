using Structor.Features.Users.Entities;
using Structor.Infrastructure.Repositories;

namespace Structor.Features.Users.Repositories;
public interface IUserRepository : IRepository<User>
{
    Task<bool> DoesUsernameExist(string username);
    Task<bool> DoesEmailExists(string username);
    Task<User?> GetByEmailOrUsername(string loginBy)
}
