using Serilog;
using Structor.Core;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddCoreServices(builder.Configuration);


builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();


app.UseCoreApp();
app.Run();
