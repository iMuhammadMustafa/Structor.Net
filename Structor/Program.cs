using Serilog;
using Structor.Core;
using Structor.Infrastructure.Globals;

var builder = WebApplication.CreateBuilder(args);


//builder.Configuration.AddUserSecrets<Program>();
AppSettings.Configuration = builder.Configuration;


builder.Services.AddCoreServices(builder.Configuration);


builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();


var linkedinConfig = builder.Configuration.GetSection("LinkedIn");
app.UseCoreApp();
app.Run();
