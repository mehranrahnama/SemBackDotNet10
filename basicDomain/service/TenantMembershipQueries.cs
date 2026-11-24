#nullable enable
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.basicDomain.service.impl;

internal sealed class TenantMembershipQueries(SemDbContext db) : ITenantMembershipQueries
{
    public async Task<User> FindUserByUserNameAsync(string username, CancellationToken ct) =>
        await db.Users.SingleOrDefaultAsync(x => x.UserName == username, ct)
        ?? throw new global::SemBackDotNet10.Common.exception.BusinessException("error.user.idIsNotValid");

    public Task<Tenant?> FindUserDefaultTenantAsync(long userId, CancellationToken ct) =>
        (from user in db.Users
         join organization in db.Organizations on (long?)user.OrganizationId equals (long?)organization.Id
         where user.UserId == userId
         select organization.Tenant).SingleOrDefaultAsync(ct);

    public Task<Tenant?> FindByIdAndUserNameAsync(long tenantId, string username, CancellationToken ct) =>
        (from user in db.Users
         join organization in db.Organizations on (long?)user.OrganizationId equals (long?)organization.Id
         where user.UserName == username && organization.TenantId == tenantId
         select organization.Tenant).SingleOrDefaultAsync(ct);
}
