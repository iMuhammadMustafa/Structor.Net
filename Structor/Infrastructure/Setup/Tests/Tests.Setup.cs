using Infrastructure.DatabaseContext;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Structor.Infrastructure.Entities;
using Structor.Infrastructure.Repositories;

namespace Structor.Infrastructure.Setup.Tests
{
    public class SqliteDbContextSetupFixture<TContext> : IDisposable
        where TContext : DbContext
    {

        //private CoreDbContext Context { get; set; } = null!;
        private string DefaultSqLiteMemoryConnectionString = $"Data Source={Guid.NewGuid()};Mode=Memory;";
        private SqliteConnection _connection { get; set; } = null!;
        public TContext CreateContextForSQLiteInMemory(string? connectionString = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) { connectionString = DefaultSqLiteMemoryConnectionString; }
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
            var _options = new DbContextOptionsBuilder()
                    .UseSqlite(_connection)
                    .Options;
            var context = (TContext)Activator.CreateInstance(typeof(TContext), _options)!;
            //var context = new TContext(_options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
            }
        }
    }


    public class BaseSetupFixture
    {
        public IConfigurationRoot Configuration { get; set; }
        public BaseSetupFixture()
        {
            // 1. Get the configuration object from  appsettings.json
            Configuration = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json")
                                .Build();
        }
    }

    public class DependencySetupFixture<TContext>
    where TContext : DbContext
    {
        public string SqlLiteInMemoryConnection = "ConnectionStrings:SqlLiteInMemoryDatabase";
        public ServiceProvider ServiceProvider { get; private set; }
        public ServiceCollection ServiceCollection { get; private set; }

        public DependencySetupFixture()
        {
            // 1. Get the configuration object from  appsettings.json
            var _configuration = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json")
                                .Build();

            // 2. Open a connection with the Sqlite in memory database. 
            // We give it a new guid so we can have a new database for each suit
            var _connection = new SqliteConnection($"Data Source={Guid.NewGuid()};Mode=Memory;");
            _connection.Open();


            // 3. Construct a DbContext options to instatciate a new context to ensure the database is created and migrations used
            var _options = new DbContextOptionsBuilder()
                    .UseSqlite(_connection)
                    .Options;
            //using var context = new CoreDbContext(_options);
            using var context = (TContext)Activator.CreateInstance(typeof(TContext), _options)!;
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();


            // 4.The dependency injection part
            //      a. We create a new service collection
            //      b. We add our DbContext
            //      c. Register the Repository from the Interface
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddDbContext<TContext>(options => options.UseSqlite(_connection));
            //ServiceCollection.AddScoped<IRepo, Repo>();
            //ServiceCollection.AddScoped(typeof(IRepository<>), typeof(Repository<,>));
            //ServiceCollection.AddScoped(typeof(IRepository<>), typeof(Repository<TestEntity, CoreDbContext>));
            ServiceCollection.AddScoped(typeof(IRepository<TestEntity>), typeof(Repository<TestEntity, TContext>));
            //ServiceCollection.AddScoped<IRepository<TestEntity>, Repository<TestEntity, TContext>>();
            //    (typeof(IRepository<TestEntity>), typeof(Repository<TestEntity, TContext>));

            // 5. Provide the collection
            ServiceProvider = ServiceCollection.BuildServiceProvider();
        }
    }
}


namespace Infrastructure.DatabaseContext
{
    public partial class CoreDbContext : DbContext
    {
        public DbSet<TestEntity> TestEntities { get; set; }
    }

    public class TestEntity : IEntity
    {

    }

}
