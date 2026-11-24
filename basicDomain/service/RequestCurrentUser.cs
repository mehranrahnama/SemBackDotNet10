using SemBackDotNet10.SecurityDomain.Infrastructure.Security;

namespace SemBackDotNet10.service.impl;

internal sealed class RequestCurrentUser(UserContext userContext, IHttpContextAccessor accessor) : ICurrentUser
{
    public long Id => accessor.HttpContext?.User.Identity?.IsAuthenticated == true
        ? userContext.UserId
        : throw new UnauthorizedAccessException("An authenticated user is required.");

    public bool IsAdmin => accessor.HttpContext?.User is { } principal
        && principal.Identity?.IsAuthenticated == true
        && principal.IsInRole("Administrators");
}
