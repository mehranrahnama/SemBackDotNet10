using Microsoft.AspNetCore.Http;
namespace Common.config.filter;
public sealed class AdditionalRegistrationFilter(RequestDelegate next,string[] authWhitelist,string contextPath)
{
    public async Task InvokeAsync(HttpContext context,ISemTokenProvider tokens,ICurrentSemUser users)
    {
        var path=(context.Request.PathBase+context.Request.Path).Value ?? "";
        var exempt=authWhitelist.Any(p=>path.StartsWith((contextPath+p).Replace("**",""),StringComparison.Ordinal));
        // Java's blacklist/session-key validation and profile-completed check are disabled in source.
        if(!exempt && tokens.GetJwtToken(context) is not null && !users.HasCurrentUser)
        {
            context.Response.StatusCode=403;
            await context.Response.WriteAsync("error.profile.notCompleted");
            return;
        }
        await next(context);
    }
}
