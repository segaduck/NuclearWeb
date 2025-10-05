using System.Diagnostics;

namespace NuclearWeb.API.Middleware;

/// <summary>
/// 請求/回應日誌記錄中介軟體
/// Request/response logging middleware
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestMethod = context.Request.Method;
        var requestPath = context.Request.Path;
        var requestQueryString = context.Request.QueryString;

        // Log request
        _logger.LogInformation(
            "HTTP {Method} {Path}{QueryString} started",
            requestMethod,
            requestPath,
            requestQueryString);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            // Log response
            var logLevel = statusCode >= 500 ? LogLevel.Error :
                           statusCode >= 400 ? LogLevel.Warning :
                           LogLevel.Information;

            _logger.Log(
                logLevel,
                "HTTP {Method} {Path}{QueryString} responded {StatusCode} in {ElapsedMilliseconds}ms",
                requestMethod,
                requestPath,
                requestQueryString,
                statusCode,
                elapsedMilliseconds);
        }
    }
}
