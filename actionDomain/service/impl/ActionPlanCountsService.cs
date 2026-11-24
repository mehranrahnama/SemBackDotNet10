using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.service.impl;

public sealed record ActionPlanCountsInput(long? LevelId, long? PlanId, long? ObjectId, long? Status);
public sealed record ActionPlanCountsResult(IReadOnlyList<string?> DeletedActionPlan, long Deleted, long Available);

public sealed class ActionPlanCountsService(ServiceRuntime runtime)
{
    public async Task<ActionPlanCountsResult> GetAsync(ActionPlanCountsInput input, CancellationToken ct)
    {
        await new PlaningLevelUserServiceImpl(runtime).HasAccessInLevelIdAsync(input.LevelId, runtime.User.Id, ct);

        var matched = runtime.Db.Database.SqlQuery<CountRow>($"""
            SELECT CAST(a.isdeleted AS bit) AS IsDeleted,
                   CAST(a.confirmstatus AS bigint) AS ConfirmStatus,
                   c.description AS Description
            FROM dbo.tblactionplan a
            JOIN dbo.tblplanperiod p ON p.planid = a.planid
            JOIN dbo.tblstrategyobject o ON o.strategyobjectid = a.strategyobjectid
            LEFT JOIN dbo.tblactionplancatalog c ON c.actionplancatalogid = a.actionplancatalogid
            WHERE a.levelid = {input.LevelId} AND p.planid = {input.PlanId}
              AND o.strategyobjectid = {input.ObjectId}
              AND ({input.Status} IS NULL OR a.confirmstatus = {input.Status})
            """);
        var available = await matched.LongCountAsync(row => row.IsDeleted == false, ct);
        var names = await matched
            .Where(row => row.IsDeleted == true && row.ConfirmStatus != null && row.ConfirmStatus != 50)
            .Select(row => row.Description).ToListAsync(ct);
        return new ActionPlanCountsResult(names, names.Count, available);
    }

    private sealed class CountRow
    {
        public bool? IsDeleted { get; set; }
        public long? ConfirmStatus { get; set; }
        public string? Description { get; set; }
    }
}
