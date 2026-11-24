using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Base.repository;
using EvalLevel = SemBackDotNet10.evaluationDomain.model.entity.EvaluationTeamHeaderLevel;
using EvalTeam = SemBackDotNet10.evaluationDomain.model.entity.EvaluationTeam;

namespace SemBackDotNet10.evaluationDomain.repository;

public sealed class EvaluationTeamHeaderLevelRepository(SemDbContext db)
    : SemRepository<EvalLevel, long?>(db), IEvaluationTeamHeaderLevelRepository
{
    private IQueryable<EvalLevel> Levels => DbSet.Include(x => x.PlaningLevel)
        .Include(x => x.EvaluationTeamHeader).ThenInclude(x => x.PlanPeriod);

    public override Task<EvalLevel?> FindByIdAsync(long? id, CancellationToken ct = default) =>
        Levels.SingleOrDefaultAsync(x => x.Id == id, ct);

    public EvalLevel findFirstByPlanIdAndLevelIdAndMonth(long? planId, long? levelId, long? month) =>
        planId is null || levelId is null || month is null ? null! : Levels.SingleOrDefault(x => x.PlaningLevel.Id == levelId && x.EvaluationTeamHeader.PlanPeriod.PlanId == planId &&
            x.EvaluationTeamHeader.FromMonth <= month && x.EvaluationTeamHeader.ToMonth >= month)!;

    public EvalLevel findFirstByPlanIdAndLevelId_hosco(long? planId, long? levelId) =>
        planId is null || levelId is null ? null! : Levels.SingleOrDefault(x => x.PlaningLevel.Id == levelId && x.EvaluationTeamHeader.PlanPeriod.PlanId == planId)!;

    public List<EvalLevel> findAllIdByEvaluationTeamHeaderId(long? id) =>
        id is null ? [] : Levels.Where(x => x.EvaluationTeamHeader.Id == id).ToList();

    public EvalLevel findByPlanIdAndMonthAndLevelId(long? planId, long? fromMonth, long? toMonth, long? levelId) =>
        planId is null || levelId is null || fromMonth is null || toMonth is null ? null! : Levels.SingleOrDefault(x => x.PlaningLevel.Id == levelId && x.EvaluationTeamHeader.PlanPeriod.PlanId == planId &&
            x.EvaluationTeamHeader.FromMonth == fromMonth && x.EvaluationTeamHeader.ToMonth == toMonth)!;

    public List<long?> getCurrentUserLevelIds(long? userId) => UserLevels(userId, null, false);
    public List<long?> findLevelIdsByUserIdAndPlanId(long? userId, long? planId) => UserLevels(userId, planId, true);

    private List<long?> UserLevels(long? userId, long? planId, bool filterPlan)
    {
        if (userId is null || (filterPlan && planId is null)) return [];
        return DbSet.Where(level => (!filterPlan || level.EvaluationTeamHeader.PlanPeriod.PlanId == planId) &&
                (level.EvaluationTeamHeader.MasterRater.UserId == userId || db.Set<EvalTeam>().Any(team =>
                    team.EvaluationTeamHeaderLevel.Id == level.Id && team.Rater.UserId == userId)))
            .Select(level => (long?)level.PlaningLevel.Id).Distinct().ToList();
    }
}
