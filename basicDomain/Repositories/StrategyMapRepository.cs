#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SemBackDotNet10.baseDomain.repository;

public interface IStrategyMapRepository : ISemRepository<StrategyMap, long>
{
    Task<List<StrategyMap>> StrategyMapReportAsync(long planId, long levelId, long month, CancellationToken ct = default);
}

public sealed class StrategyMapRepository(DbContext db)
    : SemRepository<StrategyMap, long>(db), IStrategyMapRepository
{
    public Task<List<StrategyMap>> StrategyMapReportAsync(long planId, long levelId, long month, CancellationToken ct = default)
    {
        return Set.FromSqlInterpolated($"EXEC [dbo].[Report_StrategyMap] {planId}, {levelId}, {month}").ToListAsync(ct);
    }
}
