/// <summary>
/// معادل SimpleCorsFilter جاوا (javax.servlet Filter).
/// در ASP.NET Core روش استاندارد AddCors است؛ این Middleware منطق دستی جاوا را بازسازی می‌کند.
/// </summary>
public class SimpleCorsFilter : Microsoft.AspNetCore.Http.IMiddleware
{
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    private static readonly string[] AllowedMethods = { "GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS" };
    private static readonly string[] AllowedHeaders =
        { "Authorization", "Content-Type", "X-Requested-With", "X-Tenant-Id", "X-WS-Auth", "Cache-Control" };

    public SimpleCorsFilter(Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context, Microsoft.AspNetCore.Http.RequestDelegate next)
    {
        var allowedOrigins = _configuration.GetSection("Sem:Cors:AllowedOrigins").GetChildren()
            .Select(c => c.Value).Where(v => !string.IsNullOrEmpty(v)).ToArray();

        var origin = context.Request.Headers["Origin"].ToString();

        if (!string.IsNullOrEmpty(origin))
        {
            // اگر لیست خالی باشد همه مبدأها پذیرفته می‌شوند (رفتار جاوا)؛
            // در محیط عملیاتی لیست را در appsettings محدود کنید.
            var isAllowed = allowedOrigins.Length == 0 || allowedOrigins.Contains(origin);

            if (isAllowed)
            {
                context.Response.Headers["Access-Control-Allow-Origin"] = origin;
                context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
                context.Response.Headers["Access-Control-Allow-Methods"] = string.Join(", ", AllowedMethods);
                context.Response.Headers["Access-Control-Allow-Headers"] = string.Join(", ", AllowedHeaders);
                context.Response.Headers["Vary"] = "Origin";
            }
        }

        if (context.Request.Method == Microsoft.AspNetCore.Http.HttpMethods.Options)
        {
            context.Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status200OK;
            return;
        }

        await next(context);
    }
}
