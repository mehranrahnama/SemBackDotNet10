using System.Security.Claims;
using Microsoft.AspNetCore.Http;
namespace Common.config.filter;
public sealed class JWTAuthenticationFilter(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context,ISemTokenProvider tokens,ISemUserDetails users)
    {
        var path=(context.Request.PathBase+context.Request.Path).Value ?? "";
        // Correct Java's missing return: synchronization must not execute downstream twice.
        if(string.Equals(path,"/sem/api/v1/auth/SynchronizeWithAvand",StringComparison.OrdinalIgnoreCase))
        { await next(context); return; }
        try
        {
            var token=tokens.GetJwtToken(context);
            var claims=string.IsNullOrWhiteSpace(token)?null:tokens.ValidateAndParse(token);
            if(claims is null) context.User=new ClaimsPrincipal(new ClaimsIdentity());
            else if(context.User.Identity?.IsAuthenticated!=true)
            {
                var user=string.IsNullOrWhiteSpace(claims.Username)?null:users.LoadByUsername(claims.Username);
                context.User=user is null || (user.HasUser && !user.HasCurrentOrgan)
                    ?new ClaimsPrincipal(new ClaimsIdentity()):tokens.GetAuthentication(token!,user,context);
            }
        }
        catch(SemExpiredJwtException)
        {
            context.User=new ClaimsPrincipal(new ClaimsIdentity());
            if(path!="/sem/api/v1/auth/refreshtoken" && path!="/sem/api/v1/auth/logoff")
            { context.Response.StatusCode=401; await context.Response.WriteAsync("token.Expired"); return; }
        }
        catch(SemJwtException)
        {
            context.User=new ClaimsPrincipal(new ClaimsIdentity());
            context.Response.StatusCode=401; await context.Response.WriteAsync("token.Invalid"); return;
        }
        // Deliberately outside token catch: don't reinterpret downstream application errors as JWT errors.
        await next(context);
    }
}
