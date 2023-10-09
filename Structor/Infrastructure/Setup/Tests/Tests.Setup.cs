using Infrastructure.DatabaseContext;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Structor.Infrastructure.Entities;
using Structor.Infrastructure.Repositories;

namespace Structor.Infrastructure.Setup.Tests;

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

public class DbContextSetupFixture<TContext> : IDisposable
                             where TContext : DbContext
{

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




public class DependencySetupFixture<TContext>
                             where TContext : DbContext
{
    // We give it a new guid so we can have a new database for each suit
    private string SqlLiteInMemoryConnection = $"Data Source={Guid.NewGuid()};Mode=Memory;";

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
        var _connection = new SqliteConnection(SqlLiteInMemoryConnection);
        _connection.Open();


        // 3. Construct a DbContext options to insatiate a new context to ensure the database is created and migrations used
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

        // 5. Provide the collection
        ServiceProvider = ServiceCollection.BuildServiceProvider();
    }

}



public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions options) : base(options) { }
    public DbSet<TestEntity> TestEntities { get; set; }
}

public class TestEntity : IEntity
{

}
