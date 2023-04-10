using Core;
using Core.Globals;

var builder = WebApplication.CreateBuilder(args);


AppSettings._configuration = builder.Configuration;


builder.Services.AddCoreServices();

var app = builder.Build();

app.UseCoreApp();
app.Run();
