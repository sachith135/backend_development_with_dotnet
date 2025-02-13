namespace WebApplication1.MIddleware
{
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation($"HTTP Request: {context.Request.Method} {context.Request.Path}");
            await _next(context);
            _logger.LogInformation($"HTTP Response: {context.Response.StatusCode}");
        }
    }

    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }

}
