using SemBackDotNet10.SecurityDomain.Interfaces;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Security;

public sealed class SemAuthorizationService(UserContext userContext) : IAuthorizationService
{
    public bool CheckAdminRole()
    {
        if (!userContext.IsAdministrator)
            raiseUnauthorized("دسترسی مدیریت مورد نیاز است.");

        return true;
    }

    public Task<bool> CheckAccessAsync(string uri, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(uri))
            throw new ArgumentException("آدرس URI الزامی است.", nameof(uri));

        cancellationToken.ThrowIfCancellationRequested();

        if (!userContext.IsAdministrator)
            raiseUnauthorized("دسترسی به این منبع مجاز نیست.");

        return Task.FromResult(true);
    }

    private static void raiseUnauthorized(string message)
    {
        throw new UnauthorizedAccessException(message);
    }
}
