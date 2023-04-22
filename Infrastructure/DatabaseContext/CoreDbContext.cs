using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DatabaseContext;
public partial class CoreDbContext : DbContext
{
    public CoreDbContext(DbContextOptions options) : base(options) { }
}
