using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


}
