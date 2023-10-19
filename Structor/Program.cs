using Serilog;
using Structor.Core;
using Structor.Core.Globals;

var builder = WebApplication.CreateBuilder(args);


//builder.Configuration.AddUserSecrets<Program>();
//AppSettings.Configuration = builder.Configuration;


builder.Services.AddCoreServices(builder.Configuration);


builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();


app.UseCoreApp();
app.Run();
