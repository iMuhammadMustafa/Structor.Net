using Structor.Core.Middlewares;

namespace Structor.Core;

public static class CoreAppCollection
{

    public static WebApplication UseCoreApp(this WebApplication app)
    {

        app.UseMiddleware<GlobaExceptionsMiddleware>();



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
