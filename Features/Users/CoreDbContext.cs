using Microsoft.EntityFrameworkCore;
using UserFeature.Entities;

namespace Infrastructure.DatabaseContext;
public partial class CoreDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
}
