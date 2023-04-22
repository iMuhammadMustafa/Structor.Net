using Structor.Core;
using Structor.Core.Globals;

var builder = WebApplication.CreateBuilder(args);


AppSettings._configuration = builder.Configuration;


builder.Services.AddCoreServices(builder.Configuration);



var app = builder.Build();

app.UseCoreApp();
app.Run();
