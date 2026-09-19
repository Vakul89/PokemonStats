using System.Net;
using System.Text.Json;

namespace Backend.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
            // Handle non-exceptional error status codes that have no body
            if (context.Response.StatusCode >= 400 && context.Response.ContentLength == null)
            {
                var status = context.Response.StatusCode;
                var error = new ErrorDetails
                {
                    Status = status,
                    Title = GetDefaultTitle(status),
                    Detail = null
                };

                context.Response.ContentType = "application/problem+json";
                var json = JsonSerializer.Serialize(error);
                await context.Response.WriteAsync(json);
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        (int statusCode, string? title) = exception switch
        {
            ArgumentException => ((int)HttpStatusCode.BadRequest, "Bad Request"),
            UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, "Unauthorized"),
            KeyNotFoundException => ((int)HttpStatusCode.NotFound, "Not Found"),
            _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

        var error = new ErrorDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message
        };

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(error, options));
    }

    private static string GetDefaultTitle(int statusCode) => statusCode switch
    {
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        500 => "An unexpected error occurred",
        _ => "Error"
    };
}

// Minimal ProblemDetails implementation to avoid extra references when targeting minimal APIs
internal class ErrorDetails
{
    public int? Status { get; set; }
    public string? Title { get; set; }
    public string? Detail { get; set; }
}
