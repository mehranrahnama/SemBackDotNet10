using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.measureDomain.service.impl;

// Requires the caller's authorized deletion transaction. Run before marking the current measure deleted.
internal sealed class PreviousDeletedMeasureCleanup(SemDbContext db)
{
    internal async Task ExecuteAsync(long currentMeasureId, long catalogId, long strategyObjectId, CancellationToken ct)
    {
        if (db.Database.CurrentTransaction is null)
            throw new InvalidOperationException("Previous measure cleanup requires an active EF transaction.");

        var previousId = await db.Database.SqlQuery<long>($"""
            SELECT CAST(MeasureId AS bigint) AS Value
            FROM dbo.tblDimMeasure WITH (UPDLOCK, HOLDLOCK)
            WHERE DimMeasureCatalogId = {catalogId}
              AND StrategyObjectId = {strategyObjectId}
              AND IsDeleted = 1 AND MeasureId <> {currentMeasureId}
            """).Select(id => (long?)id).SingleOrDefaultAsync(ct);
        if (previousId is not long id) return;

        await db.Database.ExecuteSqlInterpolatedAsync($"""
            DELETE FROM dbo.tbldimmeasureactplanlog WHERE dimmeasureid = {id};
            DELETE FROM dbo.tbldimmeasureactplan WHERE dimmeasureid = {id};
            DELETE FROM dbo.tbldimmeasurereplanlog WHERE measureid = {id};
            DELETE FROM dbo.tblDimMeasure WHERE MeasureId = {id} AND IsDeleted = 1;
            """, ct);
    }
}
