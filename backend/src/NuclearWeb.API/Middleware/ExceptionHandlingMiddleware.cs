using System.Net;
using System.Text.Json;

namespace NuclearWeb.API.Middleware;

/// <summary>
/// 全域例外處理中介軟體
/// Global exception handling middleware
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, errorCode, message) = exception switch
        {
            UnauthorizedAccessException => (HttpStatusCode.Forbidden, "FORBIDDEN", "您沒有權限執行此操作"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "NOT_FOUND", "找不到請求的資源"),
            ArgumentException => (HttpStatusCode.BadRequest, "INVALID_ARGUMENT", exception.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, "INVALID_OPERATION", exception.Message),
            _ => (HttpStatusCode.InternalServerError, "INTERNAL_ERROR", "伺服器發生錯誤，請稍後再試")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            error = new
            {
                code = errorCode,
                message,
                details = context.Request.HttpContext.RequestServices
                    .GetRequiredService<IWebHostEnvironment>()
                    .IsDevelopment() ? exception.StackTrace : null
            }
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
