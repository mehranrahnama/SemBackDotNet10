#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.baseDomain.repository;

public interface IReportMeasureDataByStrategyObjectIdRepository : ISemRepository<ReportMeasureDataByStrategyObjectId, long>
{
    Task<List<ReportMeasureDataByStrategyObjectId>> ReportMeasureDataByStrategyObjectIdAsync(long strategyObjectId, long planId, long levelId, long month, CancellationToken ct = default);
    Task<List<ReportMeasureDataByStrategyObjectId>> ReportMeasureDataByStrategyObjectIdAllMonthAsync(long strategyObjectId, long planId, long levelId, CancellationToken ct = default);
}

public sealed class ReportMeasureDataByStrategyObjectIdRepository(DbContext db)
    : SemRepository<ReportMeasureDataByStrategyObjectId, long>(db), IReportMeasureDataByStrategyObjectIdRepository
{
    public Task<List<ReportMeasureDataByStrategyObjectId>> ReportMeasureDataByStrategyObjectIdAsync(long strategyObjectId, long planId, long levelId, long month, CancellationToken ct = default)
    {
        return Set.FromSqlInterpolated($"EXEC [dbo].[Report_Measure_Hosco_WithStrategyObjectId] {strategyObjectId}, {planId}, {levelId}, {month}").ToListAsync(ct);
    }

    public Task<List<ReportMeasureDataByStrategyObjectId>> ReportMeasureDataByStrategyObjectIdAllMonthAsync(long strategyObjectId, long planId, long levelId, CancellationToken ct = default)
    {
        return Set.FromSqlInterpolated($"EXEC [dbo].[Report_Measure_Hosco_WithStrategyObjectIdAllMonth] {strategyObjectId}, {planId}, {levelId}").ToListAsync(ct);
    }
}
