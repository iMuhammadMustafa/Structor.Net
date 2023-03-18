namespace Structor.Net.Core;

public static class CoreAppCollection
{

    public static WebApplication UseCoreApp(this WebApplication app)
    {

        app.UseModulesServices();
        app.UseSwagger();
        app.UseSwaggerUI();

        if (app.Environment.IsDevelopment())
        {
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();


        return app;
    }

    public static WebApplication UseModulesServices(this WebApplication app)
    {

        return app;
    }
}
