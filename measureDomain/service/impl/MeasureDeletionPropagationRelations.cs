using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.measureDomain.service.impl;

// The caller must authorize the source measure and own the workflow transaction.
internal sealed class MeasureDeletionPropagationRelations(SemDbContext db)
{
    internal async Task<IReadOnlySet<long>> RemoveAsync(long catalogId, long planId, bool isAdmin, CancellationToken ct)
    {
        if (db.Database.CurrentTransaction is null)
            throw new InvalidOperationException("Removing propagation relations requires the deletion transaction.");

        var scope = await new MeasureCatalogDeletionScopeReader(db)
            .ReadPropagationScopeAsync(catalogId, planId, isAdmin, ct);

        foreach (var id in scope)
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"""
                DELETE FROM dbo.tbldimmeasurecatalogrelation
                WHERE planid = {planId}
                  AND (mainmeasurecatid = {id} OR relatedmeasurecatid = {id})
                """, ct);
        }

        return scope;
    }
}
