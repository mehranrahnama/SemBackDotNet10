using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.measureDomain.service.impl;

// Call before marking the remaining plans deleted. The caller owns the transaction
// covering cleanup and the rest of the deletion operation.
internal sealed class MeasureDeletionPlanCleanup(SemDbContext db)
{
    internal async Task ExecuteAsync(long measureId, CancellationToken ct)
    {
        if (db.Database.CurrentTransaction is null)
            throw new InvalidOperationException("Measure plan cleanup requires an active EF transaction.");

        // The Java deletion service removes all plan logs for the measure, not just deleted logs.
        await db.Database.ExecuteSqlInterpolatedAsync($"""
            DELETE FROM dbo.tbldimmeasureactplanlog WHERE dimmeasureid = {measureId};

            DELETE e
            FROM eval.tblevaluationmeasure AS e
            INNER JOIN dbo.tbldimmeasureactplan AS p ON p.dimmeasureactplanid = e.dimmeasureactid
            WHERE p.dimmeasureid = {measureId} AND p.isdeleted = 1;

            DELETE FROM dbo.tbldimmeasureactplan
            WHERE dimmeasureid = {measureId} AND isdeleted = 1;
            """, ct);
    }
}
