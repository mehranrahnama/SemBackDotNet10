#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SemBackDotNet10.baseDomain.repository;

public interface IPlanPeriodRepository : ISemRepository<PlanPeriod, int>
{
    Task<PageResult<PlanPeriodDto>> FindAllPlanPeriodsAsync(PageRequest page, string? filter, CancellationToken ct = default);
    Task<PlanPeriod?> FindByPlanIdAsync(long plandId, CancellationToken ct = default);
    Task<PlanPeriod?> FindPlanPeriodByPlanIdAsync(long plandId, CancellationToken ct = default);
    Task<PlanPeriod?> FindByStatusTypeIsActiveAsync(CancellationToken ct = default);
    Task<int> CountActivePlanPeriodsAsync(CancellationToken ct = default);
    Task<bool> CheckActivePlanPeriodAsync(long planId, CancellationToken ct = default);
    Task<List<PlanPeriod>> FindAllByPlanIdInAsync(IReadOnlyCollection<long> planIds, CancellationToken ct = default);
}

public sealed class PlanPeriodRepository(DbContext db)
    : SemRepository<PlanPeriod, int>(db), IPlanPeriodRepository
{
    public Task<PageResult<PlanPeriodDto>> FindAllPlanPeriodsAsync(PageRequest page, string? filter, CancellationToken ct = default)
    {
        return PageAsync(Set.Where(x => filter == null || EF.Functions.Like(x.PlanId.ToString(), "%" + filter + "%") || EF.Functions.Like(x.StatusType.ToString(), "%" + filter + "%")).OrderBy(x => x.StatusType).ThenBy(x => x.PlanId).ThenBy(x => x.Id).Select(x => new PlanPeriodDto(x.Id, x.PlanId, x.StatusType)), page, ct);
    }

    public Task<PlanPeriod?> FindByPlanIdAsync(long plandId, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.PlanId == plandId, ct);
    }

    public Task<PlanPeriod?> FindPlanPeriodByPlanIdAsync(long plandId, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.PlanId == plandId, ct);
    }

    public Task<PlanPeriod?> FindByStatusTypeIsActiveAsync(CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.StatusType == 2, ct);
    }

    public Task<int> CountActivePlanPeriodsAsync(CancellationToken ct = default)
    {
        return Set.CountAsync(x => x.StatusType == 2, ct);
    }

    public Task<bool> CheckActivePlanPeriodAsync(long planId, CancellationToken ct = default)
    {
        return Set.AnyAsync(x => x.PlanId == planId && x.StatusType == 2, ct);
    }

    public Task<List<PlanPeriod>> FindAllByPlanIdInAsync(IReadOnlyCollection<long> planIds, CancellationToken ct = default)
    {
        return Set.Where(x => planIds.Contains(x.PlanId)).ToListAsync(ct);
    }
}
