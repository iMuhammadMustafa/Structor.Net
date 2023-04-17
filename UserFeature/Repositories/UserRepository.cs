using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.DatabaseContext;
using Infrastructure.Repositories;
using UserFeature.Entities;

namespace UsersFeature.Repositories;
public class UserRepository : Repository<User, CoreDbContext>, IUserRepository
{
    public UserRepository(CoreDbContext coreDbContext) : base(coreDbContext) { }


}
