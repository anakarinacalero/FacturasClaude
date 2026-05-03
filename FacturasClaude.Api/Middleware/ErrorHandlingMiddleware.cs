using System.Net;
using System.Text.Json;

namespace FacturasClaude.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate pNext, ILogger<ErrorHandlingMiddleware> pLogger)
    {
        _next = pNext;
        _logger = pLogger;
    }

    public async Task InvokeAsync(HttpContext pContext)
    {
        try
        {
            await _next(pContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for request {Method} {Path}",
                pContext.Request.Method, pContext.Request.Path);

            await WriteErrorResponseAsync(pContext, ex);
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext pContext, Exception pException)
    {
        var (statusCode, message) = pException switch
        {
            ArgumentNullException => (HttpStatusCode.BadRequest, pException.Message),
            ArgumentException => (HttpStatusCode.BadRequest, pException.Message),
            KeyNotFoundException => (HttpStatusCode.NotFound, pException.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, pException.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        pContext.Response.StatusCode = (int)statusCode;
        pContext.Response.ContentType = "application/json";

        var body = JsonSerializer.Serialize(new
        {
            status = (int)statusCode,
            error = statusCode.ToString(),
            message
        });

        await pContext.Response.WriteAsync(body);
    }
}
