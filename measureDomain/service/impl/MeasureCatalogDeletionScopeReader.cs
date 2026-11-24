using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.measureDomain.service.impl;

// Read-only preparation for the catalog deletion transaction. The caller must resolve
// the active plan and determine administrator status from the authenticated user.
internal sealed class MeasureCatalogDeletionScopeReader(SemDbContext db)
{
    internal async Task<IReadOnlySet<long>> ReadAsync(long catalogId, long activePlanId, bool isAdmin, CancellationToken ct)
    {
        var catalog = await db.Database.SqlQuery<CatalogState>($"""
            SELECT CAST(isdeleted AS bit) AS Deleted FROM dbo.tbldimmeasurecatalog
            WHERE dimmeasurecatalogid = {catalogId}
            """).SingleOrDefaultAsync(ct);
        if (catalog is null) throw new BusinessException("شاخص مورد نظر یافت نشد.");
        if (catalog.Deleted) throw new BusinessException("این شاخص در فرآیند حذف قرار دارد");

        return await ReadPropagationScopeAsync(catalogId, activePlanId, isAdmin, ct);
    }

    // Link deletion validates the source measure separately, not the catalog's deletion state.
    internal async Task<IReadOnlySet<long>> ReadPropagationScopeAsync(long catalogId, long activePlanId, bool isAdmin, CancellationToken ct)
    {
        var connected = new HashSet<long>();
        // Java walks ancestors first, then descendants from the original catalog,
        // using a shared visited set. It does not traverse siblings via a parent.
        await WalkAsync(towardParents: true);
        await WalkAsync(towardParents: false);
        if (connected.Count != 0 && !isAdmin)
            throw new BusinessException("برای حذف روابط محاسباتی شاخص به دسترسی ادمین نیاز است.");
        connected.Add(catalogId);
        return connected;

        async Task WalkAsync(bool towardParents)
        {
            var pending = new Stack<long>();
            pending.Push(catalogId);
            while (pending.TryPop(out var current))
            {
                ct.ThrowIfCancellationRequested();
                FormattableString sql = towardParents
                    ? (FormattableString)$"""
                        SELECT CAST(c.dimmeasurecatalogid AS bigint) AS Value
                        FROM dbo.tbldimmeasurecatalogrelation r
                        JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = r.mainmeasurecatid
                        WHERE r.relatedmeasurecatid = {current} AND r.planid = {activePlanId}
                        """
                    : (FormattableString)$"""
                        SELECT CAST(c.dimmeasurecatalogid AS bigint) AS Value
                        FROM dbo.tbldimmeasurecatalogrelation r
                        JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = r.relatedmeasurecatid
                        WHERE r.mainmeasurecatid = {current} AND r.planid = {activePlanId}
                        """;
                foreach (var id in await db.Database.SqlQuery<long>(sql).ToListAsync(ct))
                    if (connected.Add(id)) pending.Push(id);
            }
        }
    }

    private sealed class CatalogState
    {
        public bool Deleted { get; set; }
    }
}
