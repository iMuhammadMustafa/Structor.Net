using Serilog;
using Serilog.Events;
using Structor.Core.Middlewares;

namespace Structor.Core;

public static class CoreAppCollection
{

    public static WebApplication UseCoreApp(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionsMiddleware>();

        app.UseSerilogRequestLogging(options =>
        {
            // Customize the message template   
            options.MessageTemplate = "Request {RequestMethod} {RequestHost}{RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms - From {RemoteIpAddress}";

            // Emit debug-level events instead of the defaults
            //options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;

            //Attach additional properties to the request completion event
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                 {
                     diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                     diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                     diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
                 };
        });

        app.UseFeaturesServices();
        app.UseSwagger();
        app.UseSwaggerUI();

        if (app.Environment.IsDevelopment())
        {
        }
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseHttpsRedirection();
        app.MapControllers();


        return app;
    }

    public static WebApplication UseFeaturesServices(this WebApplication app)
    {

        return app;
    }
}
