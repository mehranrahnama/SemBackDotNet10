#nullable enable
using Microsoft.EntityFrameworkCore;
using DomainPeriod = global::SemBackDotNet10.basicDomain.Model.Entity.PlanPeriod;

namespace SemBackDotNet10.service.impl;

internal sealed partial class ExternalDomainServices(SemDbContext db, IServiceProvider services) : IExternalDomainServices
{
    public async Task<List<long>> GetUserLevelIdsAsync(string source, long userId, CancellationToken ct)
    {
        var membership = await db.PlaningLevelUsers.AsNoTracking()
            .Where(x => x.UserId == userId && x.LevelId != null)
            .Select(x => x.LevelId!.Value).ToListAsync(ct);
        List<long> additional;
        switch (source)
        {
            case "PlaningLevelUser":
            case "object":
            case "perspective":
                return membership.Distinct().ToList();
            case "action":
                additional = await db.Database.SqlQuery<long>($"SELECT DISTINCT CAST([levelid] AS bigint) AS [Value] FROM [dbo].[tblactionplan] WHERE [payeshgar] = {userId} AND [levelid] IS NOT NULL").ToListAsync(ct);
                break;
            case "measure":
                additional = await db.Database.SqlQuery<long>($"SELECT DISTINCT CAST([levelid] AS bigint) AS [Value] FROM [dbo].[tbldimmeasurecatalog] WHERE [tashilgar] = {userId} AND [levelid] IS NOT NULL").ToListAsync(ct);
                break;
            case "evaluation":
                additional = await db.Database.SqlQuery<long>($"SELECT DISTINCT CAST(l.[levelid] AS bigint) AS [Value] FROM [eval].[tblevaluationteamheaderlevel] l JOIN [eval].[tblevaluationteamheader] h ON h.[evaluationteamheaderid] = l.[evaluationteamheaderid] WHERE l.[levelid] IS NOT NULL AND (h.[masterrater] = {userId} OR EXISTS (SELECT 1 FROM [eval].[tblevaluationteam] t WHERE t.[evaluationteamheaderlevelid] = l.[evaluationteamheaderlevelid] AND t.[Rater] = {userId}))").ToListAsync(ct);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(source), source, "Unknown level source.");
        }
        return membership.Union(additional).ToList();
    }

    public async Task<User> GetUserAsync(long id, CancellationToken ct) =>
        await db.Users.SingleOrDefaultAsync(user => user.UserId == id, ct)
        ?? throw new global::SemBackDotNet10.Common.exception.BusinessException("error.user.idIsNotValid");

    public async Task EnsureLevelMembershipAsync(long userId, long levelId, CancellationToken ct)
    {
        if (db.PlaningLevelUsers.Local.Any(x => x.UserId == userId && x.LevelId == levelId)
            || await db.PlaningLevelUsers.AnyAsync(x => x.UserId == userId && x.LevelId == levelId, ct)) return;
        var user = await GetUserAsync(userId, ct);
        var level = await db.PlaningLevels.SingleOrDefaultAsync(x => x.Id == levelId, ct)
            ?? throw new global::SemBackDotNet10.Common.exception.BusinessException("error.PlaningLevel.idIsNotValid");
        var now = DateTime.UtcNow;
        db.PlaningLevelUsers.Add(new PlaningLevelUser { User = user, UserId = userId, Level = level, LevelId = levelId, CreatedDate = now, ModifiedDate = now });
    }

    public Task<DomainPeriod?> FindActivePlanPeriodAsync(CancellationToken ct) =>
        db.PlanPeriods.SingleOrDefaultAsync(period => period.StatusType == 2, ct);

    public async Task<DomainPeriod> FindPlanPeriodByPlanIdAsync(long? planId, CancellationToken ct) =>
        planId is not null ? await db.PlanPeriods.SingleOrDefaultAsync(period => period.PlanId == planId, ct)
        ?? throw new global::SemBackDotNet10.Common.exception.BusinessException("error.planPeriod.notFound")
        : throw new global::SemBackDotNet10.Common.exception.BusinessException("error.planPeriod.notFound");

    public Task<List<StrategyObject>> DeletedStrategiesByPerspectiveAsync(long id, CancellationToken ct) =>
        db.StrategyObjects.IgnoreQueryFilters().Where(strategy => strategy.PerspectiveId == id && strategy.Deleted == true).ToListAsync(ct);

    public Task<bool> HasActiveStrategiesByPerspectiveAsync(long id, CancellationToken ct) =>
        db.StrategyObjects.AnyAsync(strategy => strategy.PerspectiveId == id, ct);
}
