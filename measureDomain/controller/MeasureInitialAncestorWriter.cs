using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Tenancy;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.measureDomain.controller;

internal sealed class MeasureInitialAncestorWriter(ServiceRuntime runtime, TenantContext tenant)
{
    public async Task CopyAsync(long measureId, long catalogId, long planId, CancellationToken ct)
    {
        if (runtime.Db.Database.CurrentTransaction is null) throw new InvalidOperationException("An active transaction is required.");
        var seen = new HashSet<long> { catalogId };
        var ancestors = new List<long>();
        var current = catalogId;
        while (true)
        {
            var parents = await runtime.Db.Database.SqlQuery<MeasureAncestorParent>($"""
                SELECT CAST(r.mainmeasurecatid AS bigint) AS CatalogId, CAST(l.tenantid AS bigint) AS TenantId
                FROM dbo.tbldimmeasurecatalogrelation r WITH (UPDLOCK, HOLDLOCK)
                LEFT JOIN dbo.tbldimmeasurecatalog c WITH (HOLDLOCK) ON c.dimmeasurecatalogid = r.mainmeasurecatid
                LEFT JOIN dbo.tblplaninglevel l WITH (HOLDLOCK) ON l.levelid = c.levelid
                WHERE r.relatedmeasurecatid = {current} AND r.planid = {planId}
                """).Take(2).ToListAsync(ct);
            if (parents.Count == 0) break;
            if (parents.Count > 1) throw new MeasureAncestorConflictException("شاخص نمی‌تواند بیش از یک والد داشته باشد.");
            var parent = parents[0];
            if (parent.TenantId != tenant.TenantId) throw new MeasureAncestorConflictException("رابطهٔ شاخص به سازمان دیگری متصل است.");
            if (!seen.Add(parent.CatalogId)) throw new MeasureAncestorConflictException("ساختار روابط شاخص دارای حلقه است.");
            ancestors.Add(parent.CatalogId);
            current = parent.CatalogId;
        }

        var targets = new List<long>();
        foreach (var ancestor in ancestors)
        {
            var measures = await runtime.Db.Database.SqlQuery<long>($"""
                SELECT CAST(m.measureid AS bigint) AS Value FROM dbo.tbldimmeasure m WITH (UPDLOCK, HOLDLOCK)
                WHERE m.dimmeasurecatalogid = {ancestor} AND m.planid = {planId} AND m.isdeleted = 0
                """).OrderBy(id => id).ToListAsync(ct);
            targets.AddRange(measures);
        }
        if (targets.Count == 0) return;

        var source = await ReadMonths(measureId, ct);
        if (!Complete(source) || source.Any(row => row.TaskUrl is not null))
            throw new MeasureAncestorConflictException("برنامهٔ اولیهٔ شاخص کامل یا معتبر نیست.");

        var existingTargets = new HashSet<long>();
        foreach (var targetId in targets)
        {
            var rows = await ReadMonths(targetId, ct);
            if (rows.Count == 0) continue;
            if (!Complete(rows)) throw new MeasureAncestorConflictException("برنامهٔ شاخص بالادستی باید دوازده ماه یکتا داشته باشد.");
            existingTargets.Add(targetId);
        }

        foreach (var targetId in targets)
        {
            if (existingTargets.Contains(targetId))
                await runtime.Db.Database.ExecuteSqlInterpolatedAsync($"""
                    UPDATE target SET target.planvalue = source.planvalue, target.monthplanvalue = source.monthplanvalue,
                      target.actualvalue = source.actualvalue, target.active = source.active, target.dailyvalue = source.dailyvalue,
                      target.measurepcn = source.measurepcn, target.deviation = source.deviation, target.correction = source.correction,
                      target.correctionstartdateaction = source.correctionstartdateaction,
                      target.correctiondeadlinedate = source.correctiondeadlinedate, target.correctionexecuter = source.correctionexecuter,
                      target.correctionactualvalue = source.correctionactualvalue, target.royalvisionactid = source.royalvisionactid,
                      target.confirmstatus = source.confirmstatus, target.taskurl = source.taskurl,
                      target.modifieddate = {DateTime.UtcNow}, target.modifiedby = {runtime.User.Id},
                      target.version = COALESCE(target.version, 0) + 1
                    FROM dbo.tbldimmeasureactplan target
                    JOIN dbo.tbldimmeasureactplan source ON source.month = target.month
                    WHERE target.dimmeasureid = {targetId} AND target.isdeleted = 0
                      AND source.dimmeasureid = {measureId} AND source.isdeleted = 0
                    """, ct);
            else
                await runtime.Db.Database.ExecuteSqlInterpolatedAsync($"""
                    INSERT INTO dbo.tbldimmeasureactplan
                      (dimmeasureid, month, planvalue, monthplanvalue, actualvalue, active, dailyvalue, measurepcn,
                       deviation, correction, correctionstartdateaction, correctiondeadlinedate, correctionexecuter,
                       correctionactualvalue, royalvisionactid, confirmstatus, isdeleted, taskurl,
                       createddate, modifieddate, createdby, modifiedby, version)
                    SELECT {targetId}, source.month, source.planvalue, source.monthplanvalue, source.actualvalue, source.active,
                           source.dailyvalue, source.measurepcn, source.deviation, source.correction, source.correctionstartdateaction,
                           source.correctiondeadlinedate, source.correctionexecuter, source.correctionactualvalue, source.royalvisionactid,
                           source.confirmstatus, 0, source.taskurl, {DateTime.UtcNow}, {DateTime.UtcNow}, {runtime.User.Id}, {runtime.User.Id}, 0
                    FROM dbo.tbldimmeasureactplan source
                    WHERE source.dimmeasureid = {measureId} AND source.isdeleted = 0
                    """, ct);
        }
    }

    private Task<List<MeasureAncestorMonth>> ReadMonths(long measureId, CancellationToken ct) =>
        runtime.Db.Database.SqlQuery<MeasureAncestorMonth>($"""
            SELECT CAST(ap.month AS int) AS Month, ap.taskurl AS TaskUrl
            FROM dbo.tbldimmeasureactplan ap WITH (UPDLOCK, HOLDLOCK)
            WHERE ap.dimmeasureid = {measureId} AND ap.isdeleted = 0
            """).ToListAsync(ct);

    private static bool Complete(IReadOnlyList<MeasureAncestorMonth> rows) => rows.Count == 12 &&
        rows.All(row => row.Month is >= 1 and <= 12) && rows.Select(row => row.Month).Distinct().Count() == 12;
}

internal sealed class MeasureAncestorConflictException(string message) : Exception(message);

public sealed class MeasureAncestorParent
{
    public long CatalogId { get; set; }
    public long? TenantId { get; set; }
}

public sealed class MeasureAncestorMonth
{
    public int? Month { get; set; }
    public string? TaskUrl { get; set; }
}
