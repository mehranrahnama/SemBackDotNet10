using Microsoft.AspNetCore.Http;
namespace Common.config.filter;
public sealed class SemSwitchUserFilter(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context,ISwitchUserAccounts accounts,ISwitchUserHost host)
    {
        var path=context.Request.Path.Value;
        if(path is not ("/api/v1/switch_user" or "/api/v1/switch_user_exit")) { await next(context); return; }
        // Require POST and an authenticated administrator even if host was misconfigured.
        if(!HttpMethods.IsPost(context.Request.Method)) { context.Response.StatusCode=405;return; }
        if(context.User.Identity?.IsAuthenticated!=true || !context.User.IsInRole("Administrators"))
        { context.Response.StatusCode=403; return; }
        if(path=="/api/v1/switch_user_exit") context.User=await host.Exit(context);
        else
        {
            var username=context.Request.Query["username"].ToString();
            if(context.Request.HasFormContentType)
                username=(await context.Request.ReadFormAsync(context.RequestAborted))["username"].ToString();
            if(string.IsNullOrWhiteSpace(username)) { context.Response.StatusCode=400;return; }
            const string suffix="_IRISA_LDAP";
            // Java blindly truncates suffix length. Only strip a real suffix here.
            var plain=username.EndsWith(suffix,StringComparison.Ordinal)?username[..^suffix.Length]:username;
            var account=accounts.FindForSwitch(username) ?? accounts.FindForSwitch(plain) ?? accounts.CreateLdapAccount(plain);
            context.User=await host.Switch(context,account);
        }
        await host.Success(context);
    }
}
