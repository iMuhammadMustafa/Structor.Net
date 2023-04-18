using Microsoft.EntityFrameworkCore;

namespace Structor.Infrastructure.DatabaseContext;
public partial class CoreDbContext : DbContext
{
    public CoreDbContext(DbContextOptions options) : base(options) { }
}
