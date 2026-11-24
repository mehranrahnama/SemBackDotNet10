using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
namespace Common.config.security;

public sealed record SemIdentity(IReadOnlyCollection<string> Roles,IReadOnlyCollection<long> RoleIds);
public sealed record SemPage(long Id,bool IsDefault);
// Must resolve identity from validated authentication, never from request headers.
public interface ISemIdentityResolver { SemIdentity? Resolve(ClaimsPrincipal principal); }
public interface ISemPageAccessStore
{
    Task<SemPage?> FindByUriAsync(string uri,CancellationToken cancellationToken);
    Task<bool> HasAnyRoleAccessAsync(long pageId,IReadOnlyCollection<long> roles,CancellationToken cancellationToken);
}
public sealed record SemReportAccessResource(string Uri, CancellationToken CancellationToken);
public sealed class SemRequestUriRequirement:IAuthorizationRequirement;
public sealed class SemRequestUriAccessHandler(ISemIdentityResolver identities,ISemPageAccessStore store)
    :AuthorizationHandler<SemRequestUriRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,SemRequestUriRequirement requirement)
    {
        if(!context.User.Identities.Any(x=>x.IsAuthenticated))return;
        var user=identities.Resolve(context.User);
        if(user is null)return;
        if(user.Roles.Contains("Administrators",StringComparer.Ordinal)){context.Succeed(requirement);return;}
        var http=context.Resource as HttpContext
            ?? (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext)?.HttpContext;
        string? uri;
        CancellationToken cancellationToken;
        if (context.Resource is SemReportAccessResource report)
        {
            uri = report.Uri;
            cancellationToken = report.CancellationToken;
        }
        else
        {
            if (http is null) return;
            var uris = http.Request.Headers["requesturi"];
            if (uris.Count != 1) return;
            uri = uris[0];
            cancellationToken = http.RequestAborted;
        }
        if (string.IsNullOrWhiteSpace(uri)) return;
        var page=await store.FindByUriAsync(uri,cancellationToken);
        // Fix Java's null dereference: unknown pages deny access.
        if(page is not null && (page.IsDefault || await store.HasAnyRoleAccessAsync(page.Id,user.RoleIds,cancellationToken)))
            context.Succeed(requirement);
    }
}
public static class SemAuthorizationRegistration
{
    // Host must register ISemIdentityResolver and ISemPageAccessStore and validated authentication.
    public static IServiceCollection AddSemRequestUriAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options=>options.AddPolicy("SemRequestUriAccess",policy=>
            policy.RequireAuthenticatedUser().AddRequirements(new SemRequestUriRequirement())));
        services.AddScoped<IAuthorizationHandler,SemRequestUriAccessHandler>();
        return services;
    }
}
