
using Common.exception;

namespace SemBackDotNet10.service.impl;

public sealed class DelegationServiceImpl(ServiceRuntime runtime) : ServiceBase<Delegation>(runtime)
{
    public Task<ServicePage<DelegationDto>> DelegationListAsync(string? description, long? alternativeUserId, DateTime? startDate, DateTime? endDate, ServicePageRequest page, CancellationToken ct = default)
        => R.Queries.DelegationsAsync(R.TenantName, R.User.Id, description, alternativeUserId, startDate, endDate, page, ct);

    public Task<Delegation> SaveDelegationAsync(Delegation delegation, CancellationToken ct = default) => R.TransactionAsync(async () =>
    {
        var userId = R.User.Id;
        var start = delegation.DelegationStartDate ?? throw new BusinessException("error.delegation.dateIsNotValid");
        var end = delegation.DelegationEndDate ?? throw new BusinessException("error.delegation.dateIsNotValid");
        var alternativeId = delegation.ToUserId ?? delegation.ToUser?.UserId ?? throw new BusinessException("error.delegation.alternativeUserIsNotValid");
        var today = R.TehranToday;
        if (userId == alternativeId) throw new BusinessException("error.delegation.userAndAlternativeUserIsNotValid");
        if (start.Date > end.Date) throw new BusinessException("error.delegation.dateIsNotValid");
        if (start.Date < today || end.Date < today) throw new BusinessException("error.delegation.dateIsBefore");
        var existing = await R.Queries.DelegationsByUserIdAsync(userId, ct);
        // Inclusive overlap covers containment too; exclude the row being updated.
        if (existing.Any(x => x.Id != delegation.Id
            && x.DelegationStartDate is DateTime existingStart && x.DelegationEndDate is DateTime existingEnd
            && existingStart.Date <= end.Date && start.Date <= existingEnd.Date))
            throw new BusinessException("error.delegation.hasSameDate");
        var user = await R.External.GetUserAsync(userId, ct);
        var alternative = await R.External.GetUserAsync(alternativeId, ct);
        Delegation result;
        if (delegation.Id > 0)
        {
            result = await GetByIdAsync(delegation.Id, ct);
            if ((result.FromUserId ?? result.FromUser?.UserId) != userId) throw new BusinessException("error.UserHasNoRoleOnThisPlanningLevel");
        }
        else result = new Delegation();
        result.FromUser = user; result.ToUser = alternative;
        result.DelegationStartDate = start.Date; result.DelegationEndDate = end.Date;
        result.DelegationStatus = start.Date <= today && today <= end.Date;
        return await SaveAsync(result, ct);
    }, ct);

    public Task<List<Delegation>> FindAllDelegationListAsync(CancellationToken ct = default) => R.Queries.AllDelegationsAsync(R.TenantName, ct);
    public Task<List<Delegation>> FindAllActiveDelegationListAsync(CancellationToken ct = default) => R.Queries.ActiveDelegationsAsync(R.TenantName, ct);
    // Invoke per tenant at local midnight, with a fresh scoped runtime; never fire-and-forget.
    public Task ActivateAndDeactivateDelegationAsync(CancellationToken ct = default) => R.TransactionAsync(async () =>
    {
        var today = R.TehranToday;
        foreach (var item in await FindAllDelegationListAsync(ct))
            item.DelegationStatus = item.DelegationStartDate is DateTime start && item.DelegationEndDate is DateTime end
                && start.Date <= today && today <= end.Date;
        await R.Db.SaveChangesAsync(ct);
    }, ct);
}
