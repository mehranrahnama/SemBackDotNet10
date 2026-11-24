using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.service.impl;

// Call only with changed titles from the authorized progress page.
internal sealed class MeasureProgressCorrectionWriter(ServiceRuntime runtime, ILogger<MeasureProgressCorrectionWriter> logger)
{
    internal async Task SaveAsync(IReadOnlyDictionary<long, string> changes, CancellationToken ct)
    {
        if (changes.Count == 0) return;
        if (runtime.Db.Database.CurrentTransaction != null)
            throw new InvalidOperationException("EMS synchronization must own its transaction boundaries.");
        await runtime.TransactionAsync(async () =>
        {
            foreach (var (id, title) in changes)
                await runtime.Db.Database.ExecuteSqlInterpolatedAsync($"""
                    UPDATE dbo.tbldimmeasureactplan SET correction = {title}
                    WHERE dimmeasureactplanid = {id} AND isdeleted = 0
                    """, ct);
        }, ct);

        foreach (var id in changes.Keys)
        {
            ct.ThrowIfCancellationRequested();
            try { await runtime.TransactionAsync(() => PropagateAsync(id, ct), ct); }
            catch (Exception ex) when (ex is not OperationCanceledException && !ct.IsCancellationRequested)
            {
                logger.LogError(ex, "Failed to propagate EMS correction for measure monthly record {Id}", id);
            }
        }
    }

    private sealed class Source
    {
        public long CatalogId { get; set; }
        public long PlanId { get; set; }
        public long? Month { get; set; }
    }

    private async Task PropagateAsync(long id, CancellationToken ct)
    {
        var source = await runtime.Db.Database.SqlQuery<Source>($"""
            SELECT CAST(m.dimmeasurecatalogid AS bigint) AS CatalogId, CAST(m.planid AS bigint) AS PlanId,
                   CAST(p.month AS bigint) AS Month
            FROM dbo.tbldimmeasureactplan AS p
            JOIN dbo.tbldimmeasure AS m ON m.measureid = p.dimmeasureid
            WHERE p.dimmeasureactplanid = {id} AND p.isdeleted = 0 AND m.addtosupremelevel = 1
            """).SingleOrDefaultAsync(ct);
        if (source?.Month is not long month) return;
        var current = source.CatalogId;
        var ancestors = new List<long>();
        var seen = new HashSet<long> { current };
        while (true)
        {
            var parents = await runtime.Db.Database.SqlQuery<long>($"""
                SELECT CAST(mainmeasurecatid AS bigint) AS Value FROM dbo.tbldimmeasurecatalogrelation
                WHERE relatedmeasurecatid = {current} AND planid = {source.PlanId}
                """).Take(2).ToListAsync(ct);
            if (parents.Count == 0) break;
            if (parents.Count > 1) throw new BusinessException("شاخص‌های محاسباتی نمی‌توانند بیش از یک والد داشته باشند.");
            current = parents[0];
            if (!seen.Add(current)) throw new BusinessException("ساختار شاخص‌ها دارای حلقه است.");
            ancestors.Add(current);
        }

        foreach (var ancestor in ancestors)
            await runtime.Db.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE target SET deviation = origin.deviation, correction = origin.correction,
                    correctionstartdateaction = origin.correctionstartdateaction,
                    correctiondeadlinedate = origin.correctiondeadlinedate,
                    correctionexecuter = origin.correctionexecuter,
                    correctionactualvalue = origin.correctionactualvalue
                FROM dbo.tbldimmeasureactplan AS target
                JOIN dbo.tbldimmeasure AS m ON m.measureid = target.dimmeasureid
                CROSS JOIN dbo.tbldimmeasureactplan AS origin
                WHERE origin.dimmeasureactplanid = {id} AND origin.isdeleted = 0
                  AND target.isdeleted = 0 AND target.month = {month}
                  AND m.dimmeasurecatalogid = {ancestor} AND m.planid = {source.PlanId}
                """, ct);
    }
}
