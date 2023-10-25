using Infrastructure.DatabaseContext;
using Structor.Features.Users.Entities;
using Structor.Infrastructure.Repositories;

namespace Structor.Features.Users.Repositories;
public class UserRepository : Repository<User, CoreDbContext>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(CoreDbContext coreDbContext, ILogger<UserRepository> logger) : base(coreDbContext, logger)
    {
        _logger = logger;
    }


    public async Task<bool> DoesUsernameExist(string username)
    {
        var user = await FindFirst(x=> x.Username == username);

        return user != null;
    }
    public async Task<bool> DoesEmailExists(string email)
    {
        var user = await FindFirst(x => x.Email == email);

        return user != null;
    }

    public async Task<User?> GetByEmailOrUsername(string loginBy)
    {
        return await FindFirst(x => x.Email.Equals(loginBy, StringComparison.OrdinalIgnoreCase) 
                                 || string.IsNullOrWhiteSpace(x.Username) && x.Username!.Equals(loginBy, StringComparison.OrdinalIgnoreCase));

    }

}
