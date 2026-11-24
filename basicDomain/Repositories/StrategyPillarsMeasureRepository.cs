#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Models.Entities;


namespace SemBackDotNet10.baseDomain.repository;

public interface IStrategyPillarsMeasureRepository : ISemRepository<StrategyPillarsMeasure, long>
{
    Task<List<StrategyPillarsMeasure>> FindAllStrategyPillarsMeasureByStrategyPillarsIdAsync(string? filter, long strategyPillarsMeasureId, CancellationToken ct = default);
    Task<StrategyPillarsMeasure?> FindByStrategyPillarsMeasureIdAsync(long strategyPillarsMeasureId, CancellationToken ct = default);
}

public sealed class StrategyPillarsMeasureRepository(DbContext db)
    : SemRepository<StrategyPillarsMeasure, long>(db), IStrategyPillarsMeasureRepository
{
    public Task<List<StrategyPillarsMeasure>> FindAllStrategyPillarsMeasureByStrategyPillarsIdAsync(string? filter, long strategyPillarsMeasureId, CancellationToken ct = default)
    {
        var query = Set.Where(x =>
            x.StrategyPillarsId == strategyPillarsMeasureId && !x.Deleted);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var pattern = "%" + filter + "%";
            query = query.Where(x =>
                EF.Functions.Like(x.MeasureName, pattern) ||
                (x.MeasurePlan != null && EF.Functions.Like(x.MeasurePlan, pattern)));
        }

        return query.Include(x => x.MeasureUnit).OrderBy(x => x.Id).ToListAsync(ct);
    }

    public Task<StrategyPillarsMeasure?> FindByStrategyPillarsMeasureIdAsync(long strategyPillarsMeasureId, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.Id == strategyPillarsMeasureId && x.Deleted == false, ct);
    }
}
