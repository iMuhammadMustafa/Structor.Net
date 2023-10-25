using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Structor.Core.Exceptions;
using Structor.Infrastructure.DTOs.REST;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Structor.Core.Middlewares;

public class GlobalExceptionsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<JsonOptions> _jsonOptions;
    private readonly ILogger<GlobalExceptionsMiddleware> _logger;

    public GlobalExceptionsMiddleware(RequestDelegate next, IOptions<JsonOptions> jsonOptions, ILogger<GlobalExceptionsMiddleware> logger)
    {
        _next = next;
        _jsonOptions = jsonOptions;
        _logger = logger;
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
        Response<string> errorObj = new();
        errorObj.WithError(ex.Message);


        _logger.LogError(ex.Message);
        _logger.LogError(ex.ToString());

        switch (ex)
        {
            case HttpException httpException:
                {
                    errorObj.WithStatusCode(httpException.StatusCode);
                    break;
                }

            case BadHttpRequestException badHttpRequestException:
                {
                    //errorObj.WithError(ex, 400);
                    errorObj.WithStatusCode(400);
                    break;
                }

            case UnauthorizedAccessException unauthorizedAccessException:
                {
                    //errorObj.WithError(ex, 401);
                    errorObj.WithStatusCode(401);
                    break;
                }

            case FileNotFoundException fileNotFoundException:
                {
                    //errorObj.WithError(ex, 404);
                    errorObj.WithStatusCode(404);
                    break;
                }
            case NotImplementedException NotImplementedException:
                {
                    //errorObj.WithError(ex, 404);
                    errorObj.WithStatusCode(404);
                    break;
                }

            default:
                {
                    //errorObj.WithError(ex, 500);
                    errorObj.WithStatusCode(500);
                    break;
                }
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorObj.StatusCode;


        await context.Response.WriteAsync(JsonSerializer.Serialize(errorObj, _jsonOptions.Value.JsonSerializerOptions));
       
    }

}
