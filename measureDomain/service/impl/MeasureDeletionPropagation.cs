using Microsoft.EntityFrameworkCore;
using global::Common.service.interf;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.measureDomain.service.impl;

// Internal propagation step. Caller authorizes the source and rolls back on failure.
internal sealed class MeasureDeletionPropagation(SemDbContext db, CartableStatusService statuses)
{
    internal async Task ExecuteAsync(long sourceId, long catalogId, long planId, bool isAdmin,
        string tenantName, string taskBaseUrl, CancellationToken ct)
    {
        if (db.Database.CurrentTransaction is null)
            throw new InvalidOperationException("Measure propagation requires the deletion transaction.");
        var softDeletion = new MeasureSoftDeletion(db, statuses);

        var scope = await new MeasureDeletionPropagationRelations(db)
            .RemoveAsync(catalogId, planId, isAdmin, ct);
        var targets = new List<long>();
        foreach (var id in scope)
        {
            var measureId = await db.Database.SqlQuery<long>($"""
                SELECT CAST(measureid AS bigint) AS Value
                FROM dbo.tbldimmeasure WITH (UPDLOCK, HOLDLOCK)
                WHERE dimmeasurecatalogid = {id} AND planid = {planId} AND isdeleted = 0
                """).Select(x => (long?)x).SingleOrDefaultAsync(ct);
            if (measureId is long activeId)
                targets.Add(activeId);
        }

        foreach (var id in targets)
        {
            await db.Dashboards.Where(d => d.MeasureId == id).ExecuteDeleteAsync(ct);
            if (id != sourceId)
                await softDeletion.ExecuteAsync(id, planId, tenantName, taskBaseUrl, ct);
        }
    }
}
