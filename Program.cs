using Structor.Net.Core;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCoreServices();

var app = builder.Build();

app.UseCoreApp();
app.Run();
