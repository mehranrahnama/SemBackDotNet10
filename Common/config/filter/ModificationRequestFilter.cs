using Microsoft.AspNetCore.Http;
using Common.config.modificationRequest;
namespace Common.config.filter;
public sealed class ModificationRequestFilter(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var original=context.Request.Body;
        try
        {
            await ModificationRequestWrapper.NormalizeAsync(context.Request,context.RequestAborted);
            await next(context);
        }
        finally
        {
            if(!ReferenceEquals(original,context.Request.Body))
            {
                await context.Request.Body.DisposeAsync();
                context.Request.Body=original;
            }
        }
    }
}
