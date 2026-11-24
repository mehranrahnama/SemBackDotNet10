#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.baseDomain.repository;

public interface IStrategyMapDashboardRepository : ISemRepository<StrategyMapDashboard, long>
{
    Task<List<StrategyMapDashboard>> StrategyMapDashboardReportAsync(long planId, long levelId, long month, CancellationToken ct = default);
}

public sealed class StrategyMapDashboardRepository(DbContext db)
    : SemRepository<StrategyMapDashboard, long>(db), IStrategyMapDashboardRepository
{
    public Task<List<StrategyMapDashboard>> StrategyMapDashboardReportAsync(long planId, long levelId, long month, CancellationToken ct = default)
    {
        return Set.FromSqlInterpolated($"EXEC [dbo].[Report_StrategyMapDashboard] {planId}, {levelId}, {month}").ToListAsync(ct);
    }
}
