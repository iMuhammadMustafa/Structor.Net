﻿using Core.Middlewares;

namespace Core;

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

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }

    public static WebApplication UseFeaturesServices(this WebApplication app)
    {

        return app;
    }
}
