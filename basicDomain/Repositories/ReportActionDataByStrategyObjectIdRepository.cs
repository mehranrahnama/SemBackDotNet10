#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.baseDomain.repository;

public interface IReportActionDataByStrategyObjectIdRepository : ISemRepository<ReportActionDataByStrategyObjectId, long>
{
    Task<List<ReportActionDataByStrategyObjectId>> ReportActionDataByStrategyObjectIdAsync(long strategyObjectId, long planId, long levelId, long month, CancellationToken ct = default);
    Task<List<ReportActionDataByStrategyObjectId>> ReportActionDataByStrategyObjectIdAllMonthAsync(long strategyObjectId, long planId, long levelId, CancellationToken ct = default);
}

public sealed class ReportActionDataByStrategyObjectIdRepository(DbContext db)
    : SemRepository<ReportActionDataByStrategyObjectId, long>(db), IReportActionDataByStrategyObjectIdRepository
{
    public Task<List<ReportActionDataByStrategyObjectId>> ReportActionDataByStrategyObjectIdAsync(long strategyObjectId, long planId, long levelId, long month, CancellationToken ct = default)
    {
        return Set.FromSqlInterpolated($"EXEC [dbo].[Report_Action_Hosco_WithStrategyObjectId] {strategyObjectId}, {planId}, {levelId}, {month}").ToListAsync(ct);
    }

    public Task<List<ReportActionDataByStrategyObjectId>> ReportActionDataByStrategyObjectIdAllMonthAsync(long strategyObjectId, long planId, long levelId, CancellationToken ct = default)
    {
        return Set.FromSqlInterpolated($"EXEC [dbo].[Report_Action_Hosco_WithStrategyObjectIdAllMonth] {strategyObjectId}, {planId}, {levelId}").ToListAsync(ct);
    }
}
