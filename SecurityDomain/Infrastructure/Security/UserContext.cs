using System.Security.Claims;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Security;

public sealed class UserContext(IHttpContextAccessor httpContextAccessor)
{
    private ClaimsPrincipal User => httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException("No HTTP user is available.");

    public long UserId => GetRequiredLong("user_id");

    public long AccountId => GetRequiredLong("account_id");

    public long TenantId => GetRequiredLong("tenant_id");

    public string Username => User.Identity?.Name
        ?? User.FindFirstValue(ClaimTypes.Name)
        ?? throw new UnauthorizedAccessException("The authenticated user has no name.");

    public bool IsAdministrator => User.IsInRole("Administrators") ||
        User.IsInRole("ADMINSTRATORS") || User.IsInRole("SEM_ADMIN");

    public IReadOnlyCollection<long> RoleIds => User.FindAll("role_id")
        .Select(claim => long.TryParse(claim.Value, out var roleId) ? (long?)roleId : null)
        .Where(roleId => roleId.HasValue)
        .Select(roleId => roleId!.Value)
        .Distinct()
        .ToArray();

    public IReadOnlyCollection<string> Permissions => User.FindAll("permission")
        .Select(claim => claim.Value)
        .Where(permission => !string.IsNullOrWhiteSpace(permission))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

    public bool HasPermission(string permission) => User.HasClaim("permission", permission);

    public bool IsSwitchUser => User.HasClaim("is_switch_user", "true");

    public long? OriginalUserId => long.TryParse(
        User.FindFirstValue("original_user_id"), out var userId) ? userId : null;

    private long GetRequiredLong(string claimType)
    {
        if (!long.TryParse(User.FindFirstValue(claimType), out var value))
            throw new UnauthorizedAccessException($"The required claim '{claimType}' is missing.");

        return value;
    }
}
