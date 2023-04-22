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
    public UserRepository(CoreDbContext coreDbContext) : base(coreDbContext) { }


}
