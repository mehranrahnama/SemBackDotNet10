using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.measureDomain.service.impl;

// Invoke only after authorizing the measure and validating the only-this-level request.
// The caller owns the transaction for the entire measure deletion workflow.
internal sealed class MeasureDeletionOutgoingRelations(SemDbContext db)
{
    internal async Task RemoveAsync(long catalogId, long planId, string tenantName, CancellationToken ct)
    {
        if (db.Database.CurrentTransaction is null)
            throw new InvalidOperationException("Removing measure relations requires the deletion transaction.");

        var lowerCatalogIds = await db.Database.SqlQuery<long>($"""
            SELECT DISTINCT CAST(r.relatedmeasurecatid AS bigint) AS Value
            FROM dbo.tbldimmeasurecatalogrelation r
            WHERE r.mainmeasurecatid = {catalogId} AND r.planid = {planId}
              AND r.relatedmeasurecatid IS NOT NULL
            """).ToListAsync(ct);

        await db.Database.ExecuteSqlInterpolatedAsync($"""
            DELETE FROM dbo.tbldimmeasurecatalogrelation
            WHERE mainmeasurecatid = {catalogId} AND planid = {planId}
            """, ct);

        if (!global::Common.utils.TenantNameConstants.IsParentOnlyTenant(tenantName)) return;

        foreach (var lowerCatalogId in lowerCatalogIds)
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE dbo.tbldimmeasure
                SET addtosupremelevel = 0, sendtoparentonly = 0
                WHERE dimmeasurecatalogid = {lowerCatalogId}
                  AND planid = {planId} AND isdeleted = 0
                """, ct);
        }
    }
}
