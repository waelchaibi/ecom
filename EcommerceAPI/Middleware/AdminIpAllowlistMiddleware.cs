using EcommerceAPI.Configuration;
using Microsoft.Extensions.Options;

namespace EcommerceAPI.Middleware;

public sealed class AdminIpAllowlistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AdminSecurityOptions _options;

    public AdminIpAllowlistMiddleware(RequestDelegate next, IOptions<AdminSecurityOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (!path.StartsWith("/api/admin", StringComparison.OrdinalIgnoreCase) ||
            _options.AllowedIps is null ||
            _options.AllowedIps.Count == 0)
        {
            await _next(context);
            return;
        }

        var remote = context.Connection.RemoteIpAddress;
        if (remote is null)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { error = "Admin access denied." });
            return;
        }

        var allowed = _options.AllowedIps.Any(ip =>
            System.Net.IPAddress.TryParse(ip, out var configured) && configured.Equals(remote));

        if (!allowed)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { error = "Admin access denied from this network." });
            return;
        }

        await _next(context);
    }
}
