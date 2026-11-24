using Microsoft.AspNetCore.Http;
using SemBackDotNet10.webserviceDomain.service.interf;
namespace Common.config.filter;
public sealed class WebServiceAuthenticationFilter(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IApiKeyService credentials)
    {
        var path=(context.Request.PathBase+context.Request.Path).Value ?? "";
        if(path.StartsWith("/sem/api/v1/webservices",StringComparison.Ordinal))
        {
            var key=context.Request.Headers["X-API-KEY"];
            var hash=context.Request.Headers["X-API-HASH"];
            if(key.Count!=1 || hash.Count!=1 || !await credentials.ValidateApiCredentialsAsync(key.ToString(), hash.ToString(), context.RequestAborted))
            { context.Response.StatusCode=401; return; }
        }
        await next(context);
    }
}
