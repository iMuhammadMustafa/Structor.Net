using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Structor.Net.Core.DatabaseContext;
using Structor.Net.Core.Globals;
using Structor.Net.Infrastructure.Temp;

namespace Structor.Net.Core.Tests;



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
        context.Database.EnsureCreated();

        return context;
    }

    public void Dispose()
    {
        if (_connection != null)
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
public class DependencySetupFixture<TContext>
    where TContext : DbContext
{
    public string SqlLiteInMemoryConnection = AppSettings.SqlLiteInMemoryConnection;
    public ServiceProvider ServiceProvider { get; private set; }

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
        //using (var context = new CoreDbContext(_options))
        using var context = (TContext)Activator.CreateInstance(typeof(TContext), _options)!;
        context.Database.EnsureCreated();


        // 4.The dependency injection part
        //      a. We create a new service collection
        //      b. We add our DbContext
        //      c. Register the Repository from the Interface
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<TContext>(options => options.UseSqlite(_connection));
        serviceCollection.AddScoped<IRepo, Repo>();

        // 5. Provide the collection
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
}
