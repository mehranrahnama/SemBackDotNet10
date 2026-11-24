using System.Security.Claims;
using Common.config.security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.Controllers;

public sealed class CheckAccessAttribute() : AuthorizeAttribute("SemRequestUriAccess");
public sealed class CheckAccessIfAdminAttribute() : AuthorizeAttribute("SemAdministrators");

internal sealed class SemClaimsIdentityResolver : ISemIdentityResolver
{
    public SemIdentity? Resolve(ClaimsPrincipal principal)
    {
        var identities = principal.Identities.Where(identity => identity.IsAuthenticated).ToArray();
        if (identities.Length == 0) return null;
        var roles = identities.SelectMany(identity => identity.FindAll(identity.RoleClaimType))
            .Select(claim => claim.Value).Distinct(StringComparer.Ordinal).ToArray();
        var ids = identities.SelectMany(identity => identity.FindAll("role_id"))
            .Select(claim => long.TryParse(claim.Value, out var id) ? (long?)id : null)
            .OfType<long>().Distinct().ToArray();
        return new SemIdentity(roles, ids);
    }
}

internal sealed class SemRepositoryPageAccessStore(AppDbContext db) : ISemPageAccessStore
{
    public async Task<SemPage?> FindByUriAsync(string uri, CancellationToken cancellationToken)
    {
        var page = await db.Database.SqlQuery<PageAccessLookup>($"SELECT CAST(PageId AS bigint) AS Id, IsDefault FROM dbo.tblPage WHERE Uri = {uri}")
            .FirstOrDefaultAsync(cancellationToken);
        return page is null ? null : new SemPage(page.Id, page.IsDefault == true);
    }

    public async Task<bool> HasAnyRoleAccessAsync(long pageId, IReadOnlyCollection<long> roles, CancellationToken cancellationToken)
    {
        if (roles.Count == 0) return false;
        var roleIds = string.Join(',', roles);
        return await db.Database.SqlQuery<int>($"SELECT 1 AS [Value] FROM dbo.tblPageAccess WHERE PageId = {pageId} AND RoleId IN (SELECT TRY_CAST(value AS int) FROM STRING_SPLIT({roleIds}, ','))")
            .AnyAsync(cancellationToken);
    }
}

internal sealed class PageAccessLookup
{
    public long Id { get; set; }
    public bool? IsDefault { get; set; }
}
