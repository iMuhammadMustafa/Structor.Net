using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Structor.Net.Infrastructure.Entities;

namespace Structor.Net.Core.DatabaseContext;
public partial class CoreDbContext : DbContext
{
    public CoreDbContext(DbContextOptions options) : base(options) { }
}
