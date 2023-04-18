using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.DatabaseContext;
using Structor.Features.UserFeature.Entities;
using Structor.Infrastructure.Repositories;

namespace Structor.Features.UserFeature.Repositories;
public class UserRepository : Repository<User, CoreDbContext>, IUserRepository
{
    public UserRepository(CoreDbContext coreDbContext) : base(coreDbContext) { }


}
