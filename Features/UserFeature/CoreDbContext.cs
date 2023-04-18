using Microsoft.EntityFrameworkCore;
using Structor.Features.UserFeature.Entities;

namespace Infrastructure.DatabaseContext;
public partial class CoreDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
}
