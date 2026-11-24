using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;
using global::Common.service.interf;
using static global::Common.utils.cartable.CartableStatusConstants;

namespace SemBackDotNet10.measureDomain.service.impl;

internal sealed class MeasureSoftDeletion(SemDbContext db, CartableStatusService statuses)
{
    internal async Task ExecuteAsync(long measureId, long planId, string tenantName, string taskBaseUrl, CancellationToken ct)
    {
        if (db.Database.CurrentTransaction is null)
            throw new InvalidOperationException("Soft deletion requires the deletion transaction.");

        var row = await db.Database.SqlQuery<DeletionRow>($"""
            SELECT CAST(dimmeasurecatalogid AS bigint) AS CatalogId,
                   CAST(strategyobjectid AS bigint) AS StrategyObjectId,
                   CAST(isplanned AS bit) AS Planned
            FROM dbo.tbldimmeasure WITH (UPDLOCK, HOLDLOCK)
            WHERE measureid = {measureId} AND planid = {planId} AND isdeleted = 0
            """).SingleOrDefaultAsync(ct);
        if (row is null) return;

        var pair = MeasureDeletionStatusPolicy.Resolve(tenantName, row.Planned, statuses);
        var status = pair.NewStatus ?? throw new InvalidOperationException("The deletion status is missing.");
        var url = MeasureDeletionTransition.BuildTaskUrl(taskBaseUrl, measureId, pair.OldStatus, status);

        await new PreviousDeletedMeasureCleanup(db).ExecuteAsync(measureId, row.CatalogId, row.StrategyObjectId, ct);
        await new MeasureDeletionPlanCleanup(db).ExecuteAsync(measureId, ct);
        await db.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE dbo.tbldimmeasureactplan
            SET isdeleted = 1, confirmstatus = {status}, taskurl = {url}, outcome = {OUTCOME_CONFIRM}
            WHERE dimmeasureid = {measureId} AND isdeleted = 0;

            UPDATE dbo.tbldimmeasure
            SET isdeleted = 1, deletefirststatus = confirmstatus, confirmstatus = {status},
                taskurl = {url}, outcome = {OUTCOME_CONFIRM}
            WHERE measureid = {measureId} AND planid = {planId} AND isdeleted = 0;
            """, ct);
    }

    private sealed class DeletionRow
    {
        public long CatalogId { get; set; }
        public long StrategyObjectId { get; set; }
        public bool Planned { get; set; }
    }
}
