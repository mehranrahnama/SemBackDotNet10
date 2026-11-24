#nullable enable
using Microsoft.EntityFrameworkCore;
using DomainTenant = global::SemBackDotNet10.basicDomain.Model.Entity.Tenant;

namespace SemBackDotNet10.service.impl;

internal sealed partial class ServiceQueries
{
    private async Task<long> ResolveTenantIdAsync(string tenant, CancellationToken ct)
    {
        if (tenantContext.HasTenant) return tenantContext.TenantId;
        if (string.IsNullOrWhiteSpace(tenant)) throw new InvalidOperationException("A tenant is required.");
        return await db.Set<DomainTenant>().Where(x => x.TenantCode == tenant || x.TenantName == tenant)
            .Select(x => (long?)x.Id).SingleOrDefaultAsync(ct)
            ?? throw new InvalidOperationException("The delegation tenant could not be resolved.");
    }

    public async Task<ServicePage<DelegationDto>> DelegationsAsync(string tenant, long userId, string? description, long? alternativeUserId, DateTime? startDate, DateTime? endDate, ServicePageRequest page, CancellationToken ct)
    {
        var tenantId = await ResolveTenantIdAsync(tenant, ct);
        var query = db.Delegations.AsNoTracking().Where(x => x.FromUserId == userId && x.FromUser != null && x.FromUser.TenantId == tenantId);
        if (alternativeUserId.HasValue) query = query.Where(x => x.ToUserId == alternativeUserId);
        if (startDate.HasValue) query = query.Where(x => x.DelegationEndDate >= startDate);
        if (endDate.HasValue) query = query.Where(x => x.DelegationStartDate <= endDate);
        if (description is not null)
        {
            var pattern = $"%{description}%";
            query = query.Where(x => x.FromUser != null && EF.Functions.Like((x.FromUser.NameF ?? "") + " " + (x.FromUser.FamilyF ?? ""), pattern)
                || x.ToUser != null && EF.Functions.Like((x.ToUser.NameF ?? "") + " " + (x.ToUser.FamilyF ?? ""), pattern));
        }
        var projected = query.Select(x => new DelegationDto
        {
            Id = x.Id, FromUserId = x.FromUserId, ToUserId = x.ToUserId,
            FromUserName = x.FromUser == null ? null : x.FromUser.UserName,
            FromUserDisplayName = x.FromUser == null ? null : (x.FromUser.NameF ?? "") + " " + (x.FromUser.FamilyF ?? ""),
            ToUserName = x.ToUser == null ? null : x.ToUser.UserName,
            ToUserDisplayName = x.ToUser == null ? null : (x.ToUser.NameF ?? "") + " " + (x.ToUser.FamilyF ?? ""),
            DelegationStartDate = x.DelegationStartDate, DelegationEndDate = x.DelegationEndDate,
            DelegationStatus = x.DelegationStatus, DelegationStatusDesc = x.DelegationStatus ? "فعال" : "غیرفعال"
        });
        return await PageProjectedAsync(projected, page, ct);
    }

    public Task<List<Delegation>> DelegationsByUserIdAsync(long userId, CancellationToken ct) =>
        db.Delegations.Where(x => x.FromUserId == userId).ToListAsync(ct);

    public async Task<List<Delegation>> AllDelegationsAsync(string tenant, CancellationToken ct)
    {
        var tenantId = await ResolveTenantIdAsync(tenant, ct);
        return await db.Delegations.Where(x => x.FromUser != null && x.FromUser.TenantId == tenantId).ToListAsync(ct);
    }

    public async Task<List<Delegation>> ActiveDelegationsAsync(string tenant, CancellationToken ct)
    {
        var tenantId = await ResolveTenantIdAsync(tenant, ct);
        return await db.Delegations.Where(x => x.FromUser != null && x.FromUser.TenantId == tenantId && x.DelegationStatus).ToListAsync(ct);
    }

    public Task<bool> HasActiveDelegationAsync(long fromId, long toId, CancellationToken ct) =>
        db.Delegations.AnyAsync(x => x.FromUserId == fromId && x.ToUserId == toId && x.DelegationStatus, ct);
}
