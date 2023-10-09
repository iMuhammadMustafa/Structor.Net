using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Structor.Infrastructure.DTOs.REST;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Structor.Core.Middlewares;

public class GlobalExceptionsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<JsonOptions> _jsonOptions;

    public GlobalExceptionsMiddleware(RequestDelegate next, IOptions<JsonOptions> jsonOptions)
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
            case NotImplementedException NotImplementedException:
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

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorObj, _jsonOptions.Value.JsonSerializerOptions));

        throw ex;
    }

}
