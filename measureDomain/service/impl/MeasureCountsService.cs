using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.service.impl;

public sealed record MeasureCountsInput(long? LevelId, long? PlanId, long? ObjectId, long? Status);
public sealed record MeasureCountsResult(IReadOnlyList<string?> DeletedMeasure, long Deleted, long Available);

public sealed class MeasureCountsService(ServiceRuntime runtime)
{
    public async Task<MeasureCountsResult> GetAsync(MeasureCountsInput input, CancellationToken ct)
    {
        await new PlaningLevelUserServiceImpl(runtime).HasAccessInLevelIdAsync(input.LevelId, runtime.User.Id, ct);

        // Catalog level, not the goal's level, is the predicate in Java.
        var matched = runtime.Db.Database.SqlQuery<MeasureCountRow>($"""
            SELECT CAST(m.isdeleted AS bit) AS IsDeleted,
                   CAST(m.confirmstatus AS bigint) AS ConfirmStatus,
                   CAST(c.dimmeasurecatalogrepositoryid AS bigint) AS RepositoryId
            FROM dbo.tbldimmeasure m
            JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
            JOIN dbo.tblstrategyobject o ON o.strategyobjectid = m.strategyobjectid
            WHERE c.levelid = {input.LevelId} AND m.planid = {input.PlanId}
              AND o.strategyobjectid = {input.ObjectId}
              AND ({input.Status} IS NULL OR m.confirmstatus = {input.Status})
            """);
        var available = await matched.LongCountAsync(row => row.IsDeleted == false, ct);
        var deleted = matched.Where(row => row.IsDeleted == true && row.ConfirmStatus != null && row.ConfirmStatus != 50);
        var repositories = runtime.Db.Database.SqlQuery<MeasureCountNameRow>($"""
            SELECT CAST(r.dimmeasurerepositoryid AS bigint) AS Id, r.measurename AS Name
            FROM dbo.tblDimMeasureReporsitory r
            """);
        var deletedCount = await deleted.LongCountAsync(ct);
        var names = await (from row in deleted
                           join repository in repositories on row.RepositoryId equals (long?)repository.Id into matches
                           from repository in matches.DefaultIfEmpty()
                           select repository.Name).ToListAsync(ct);
        return new MeasureCountsResult(names, deletedCount, available);
    }

    private sealed class MeasureCountRow
    {
        public bool? IsDeleted { get; set; }
        public long? ConfirmStatus { get; set; }
        public long? RepositoryId { get; set; }
    }

    private sealed class MeasureCountNameRow
    {
        public long Id { get; set; }
        public string? Name { get; set; }
    }
}
