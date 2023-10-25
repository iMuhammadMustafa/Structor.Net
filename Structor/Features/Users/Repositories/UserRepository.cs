using Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;
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


    public async Task<bool> UsernameExists(string username)
    {
        return await DbSet.AnyAsync(x => x.Username == username);
    }
    public async Task<bool> EmailExists(string email)
    {

        return await DbSet.AnyAsync(x => x.Email == email);
    }

    public async Task<User?> GetByEmailOrUsername(string usernameOrEmail)
    {
        return await FindFirst(x => x.Email.Equals(usernameOrEmail) 
                                 || (string.IsNullOrWhiteSpace(x.Username) && x.Username!.Equals(usernameOrEmail)));

    }

}
