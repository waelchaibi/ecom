using System.Collections.Concurrent;

namespace EcommerceAPI.Middleware;

public sealed class RateLimitMiddleware
{
    private static readonly ConcurrentDictionary<string, Window> Windows = new();

    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitMiddleware> _logger;

    public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        var key = ResolveKey(context.Request.Method, path);
        if (key is null)
        {
            await _next(context);
            return;
        }

        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var bucketKey = $"{ip}:{key}";
        var now = DateTime.UtcNow;
        var window = Windows.GetOrAdd(bucketKey, _ => new Window());

        var allowed = false;
        lock (window)
        {
            if (now - window.Start > window.Duration)
            {
                window.Start = now;
                window.Count = 0;
            }

            window.Count++;
            allowed = window.Count <= window.Limit;
        }

        if (!allowed)
        {
            _logger.LogWarning("Rate limit exceeded for {BucketKey}", bucketKey);
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsJsonAsync(new { error = "Too many requests. Please try again shortly." });
            return;
        }

        await _next(context);
    }

    private static string? ResolveKey(string method, string path)
    {
        if (path.StartsWith("/api/auth/login", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/api/auth/customer/login", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/api/auth/customer/register", StringComparison.OrdinalIgnoreCase))
            return "auth";

        if (method == HttpMethods.Post &&
            path.Equals("/api/orders", StringComparison.OrdinalIgnoreCase))
            return "orders";

        if (path.Contains("/pay", StringComparison.OrdinalIgnoreCase))
            return "pay";

        return null;
    }

    private sealed class Window
    {
        public DateTime Start { get; set; } = DateTime.UtcNow;
        public int Count { get; set; }
        public TimeSpan Duration { get; } = TimeSpan.FromMinutes(1);
        public int Limit { get; } = 30;
    }
}
