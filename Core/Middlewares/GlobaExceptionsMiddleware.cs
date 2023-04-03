using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Structor.Net.Infrastructure.DTOs.REST;

namespace Structor.Net.Core.Middlewares;

public class GlobaExceptionsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<MvcNewtonsoftJsonOptions> _jsonOptions;

    public GlobaExceptionsMiddleware(RequestDelegate next, IOptions<MvcNewtonsoftJsonOptions> jsonOptions)
    {
        _next = next;
        _jsonOptions = jsonOptions;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptions(context, ex);
        }
    }

    private async Task HandleExceptions(HttpContext context, Exception ex)
    {
        IResponse<string> errorObj = new();
        errorObj.WithMessage(ex.Message);

        switch (ex)
        {
            case BadHttpRequestException badHttpRequestException:
                {
                    errorObj.WithError(ex, 400);
                    break;
                }

            case UnauthorizedAccessException unauthorizedAccessException:
                {
                    errorObj.WithError(ex, 401);
                    break;
                }

            case FileNotFoundException fileNotFoundException:
                {
                    errorObj.WithError(ex, 404);
                    break;
                }

            default:
                {
                    errorObj.WithError(ex, 500);
                    break;
                }
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorObj.StatusCode;

        await context.Response.WriteAsync(JsonConvert.SerializeObject(errorObj, _jsonOptions.Value.SerializerSettings));

    }

}
