#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Models.Entities;


namespace SemBackDotNet10.baseDomain.repository;

public interface IStrategyPillarRepository : ISemRepository<StrategyPillar, long>
{
    Task<StrategyPillar?> GetStrategyPillarDataaByPlanIdAndLevelIdAsync(long planId, long levelId, CancellationToken ct = default);
    Task<StrategyPillar?> GetStrategyPillarDataByPlanIdAsync(long planId, CancellationToken ct = default);
    Task<StrategyPillar?> GetStrategyPillarDataaByPlanIdAsync(long planId, CancellationToken ct = default);
}

public sealed class StrategyPillarRepository(DbContext db)
    : SemRepository<StrategyPillar, long>(db), IStrategyPillarRepository
{
    public Task<StrategyPillar?> GetStrategyPillarDataaByPlanIdAndLevelIdAsync(long planId, long levelId, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.PlanId == planId && x.LevelId == levelId, ct);
    }

    public Task<StrategyPillar?> GetStrategyPillarDataByPlanIdAsync(long planId, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.PlanId == planId, ct);
    }

    public Task<StrategyPillar?> GetStrategyPillarDataaByPlanIdAsync(long planId, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.PlanId == planId, ct);
    }
}
