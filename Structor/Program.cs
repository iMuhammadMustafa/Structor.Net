using Structor.Core;
using Structor.Core.Globals;

var builder = WebApplication.CreateBuilder(args);


//builder.Configuration.AddUserSecrets<Program>();
AppSettings.Configuration = builder.Configuration;


builder.Services.AddCoreServices(builder.Configuration);



var app = builder.Build();


var linkedinConfig = builder.Configuration.GetSection("LinkedIn");
app.UseCoreApp();
app.Run();
