using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace CityAid.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path,
            Detail = exception.Message
        };

        switch (exception)
        {
            case ArgumentException:
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Bad Request";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                break;
            case UnauthorizedAccessException:
                problemDetails.Status = (int)HttpStatusCode.Forbidden;
                problemDetails.Title = "Forbidden";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
                break;
            case KeyNotFoundException:
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Title = "Not Found";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                break;
            default:
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Title = "An error occurred";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                break;
        }

        context.Response.StatusCode = problemDetails.Status.Value;

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}