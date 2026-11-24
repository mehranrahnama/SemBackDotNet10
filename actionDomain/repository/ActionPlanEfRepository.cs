#nullable enable
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Base.repository;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed class ActionPlanEfRepository(SemDbContext db) : SemRepository<ActionPlan, long?>(db), ActionPlanRepository
{
    public void updateFacilitatorAndManagerByPlanningLevelId(long? levelId, long? facilitatorId, long? managerId) =>
        db.Database.ExecuteSqlInterpolated($"UPDATE [dbo].[tblactionplan] SET payeshgar = COALESCE({facilitatorId}, payeshgar), modir = COALESCE({managerId}, modir) WHERE levelid = {levelId}");

    // The strategy-object global filter is not part of the Java repository queries.
    private IQueryable<ActionPlan> Plans => db.DomainActionPlans.IgnoreQueryFilters();
    private IQueryable<ActionPlan> ByCatalog(long? id) => Plans.Where(x => id != null && x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == id);
    private IQueryable<ActionPlan> ByObject(long? id) => Plans.Where(x => id != null && x.StrategyObject != null && x.StrategyObject.Id == id);
    private IQueryable<ActionPlan> CatalogAndPeriod(long? catalog, long? plan) =>
        ByCatalog(catalog).Where(x => plan != null && x.PlanPeriod != null && x.PlanPeriod.PlanId == plan);
    private IQueryable<ActionPlan> CatalogAndObject(long? catalog, long? obj) =>
        ByCatalog(catalog).Where(x => obj != null && x.StrategyObject != null && x.StrategyObject.Id == obj);

    public List<ActionPlan> findByStrategyObject_TitleAndAddToSupremeLevelTrue(string? title) =>
        Plans.Where(x => x.StrategyObject != null && x.StrategyObject.Title == title && x.AddToSupremeLevel).ToList();
    public List<long?> getCurrentUserLevelIds(long? id) => Plans.Where(x => id != null && x.Payeshgar != null && x.Payeshgar.UserId == id)
        .Select(x => x.PlaningLevel == null ? (long?)null : x.PlaningLevel.Id).Distinct().ToList();
    public List<ActionPlan> findAllByObjectId(long? id) => ByObject(id).ToList();
    public ActionPlan findByActionPlanCatalog_IdAndStrategyObject_IdAndStrategyObject_PlaningLevel_IdAndPlanPeriod_PlanId(long? catalog, long? obj, long? level, long? plan) =>
        CatalogAndObject(catalog, obj).SingleOrDefault(x => level != null && plan != null && x.StrategyObject!.PlaningLevelId == level && x.PlanPeriod!.PlanId == plan)!;
    public List<ActionPlan> findByCatalogId(long? id) => ByCatalog(id).ToList();
    public List<ActionPlan> findByCatalogIdAndDeleteStatusIsFalse(long? id) => ByCatalog(id).Where(x => x.DeleteStatus == false).ToList();
    public List<ActionPlan> findByCatalogIdAndDeletedIsFalseOrNotFinalState(long? id) => ByCatalog(id).Where(x => x.DeleteStatus == false || (x.Confirmstatus != null && x.Confirmstatus != 50)).ToList();
    public List<ActionPlan> findByCatalogIdAndConfirmstatus(long? id) => ByCatalog(id).Where(x => x.Confirmstatus != null && x.Confirmstatus != 50).ToList();
    public List<ActionPlan> findByActionPlanCatalogIdAndDeleteStatusIsTrue(long? id) => ByCatalog(id).Where(x => x.DeleteStatus == true && x.Confirmstatus != null && x.Confirmstatus != 50).ToList();
    public List<ActionPlan> findByActionPlanCatalogIdAndDeleted(long? id) => ByCatalog(id).Where(x => x.DeleteStatus == true).ToList();
    public List<ActionPlan> findByActionPlanCatalogIdAndDeleteStatusIsFalse(long? id, long? plan) => CatalogAndPeriod(id, plan).Where(x => x.DeleteStatus == false && x.Confirmstatus != null && x.Confirmstatus != 50).ToList();
    public List<ActionPlan> findByActionPlanCatalogIdAndStrategyObjIdAndDeleteStatusIsFalse(long? id, long? obj) => CatalogAndObject(id, obj).Where(x => x.DeleteStatus == false).ToList();
    public ActionPlan findByActionPlanCatalogIdAndStrategyObjectIdAndDeleteStatusIsTrue(long? id, long? obj) => CatalogAndObject(id, obj).SingleOrDefault(x => x.DeleteStatus == true)!;
    public ActionPlan findFirstByActionPlanCatalogIdAndStrategyObjectIdAndDeleteStatusIsFalse(long? id, long? obj) => CatalogAndObject(id, obj).SingleOrDefault(x => x.DeleteStatus == false)!;

    public ActionPlanDto findActionPlanById(long? id) => Plans.Where(x => id != null && x.Id == id)
        .Select(x => new ActionPlanDto
        {
            Id = x.Id, LevelDes = x.PlaningLevel!.DesLevel, StrategyObjectDescription = x.StrategyObject!.Title,
            ActionPlanCatalogDescription = x.ActionPlanCatalog!.Description,
            Modir = (x.Modir!.NameF ?? "") + " " + (x.Modir.FamilyF ?? ""),
            Payeshgar = (x.Payeshgar!.NameF ?? "") + " " + (x.Payeshgar.FamilyF ?? ""),
            Priority = x.Priority, Weight = x.Weight, ActionPlanCatalogId = x.ActionPlanCatalog == null ? null : x.ActionPlanCatalog.Id,
            Type = x.Type, StartDate = x.ActionPlanCatalog.StartDate, EndDate = x.ActionPlanCatalog.EndDate,
            Status = x.Confirmstatus, DeleteStatus = x.DeleteStatus, LevelId = x.PlaningLevel == null ? null : x.PlaningLevel.Id,
            SendToUpperLevels = x.AddToSupremeLevel, SendToParentOnly = x.SendToParentOnly
        }).SingleOrDefault()!;
    public List<ActionPlan> findByActionPlanCatalogIdAndPlanId(long? id, long? plan) => CatalogAndPeriod(id, plan).ToList();
    public List<ActionPlan> findByActionPlanCatalogIdAndPlanIAndDeletedIsFalse(long? id, long? plan) => CatalogAndPeriod(id, plan).Where(x => x.DeleteStatus == false).ToList();
    public long? CountActiveActions(long? plan) => db.Database.SqlQuery<long?>($"EXEC [dbo].[CountActiveActions] {plan}").AsEnumerable().SingleOrDefault();
    public long? CountActiveActionsByPlanIdLevelId(long? plan, long? level) => db.Database.SqlQuery<long?>($"EXEC [dbo].[CountActiveActionsByPlanIdLevelId] {plan}, {level}").AsEnumerable().SingleOrDefault();
    public List<ActionPlan> findByStrategyObjectIdAndDeleteStatusIsFalse(long? id) => ByObject(id).Where(x => x.DeleteStatus == false).ToList();
    public List<ActionPlan> findByActionPlanCatalogIdAndDeletedIsTrue(long? id) => findByActionPlanCatalogIdAndDeleteStatusIsTrue(id);
    public List<ActionPlan> getByActionPlanCatalogIdAndDeletedIsFalse(long? id) => findByCatalogIdAndDeleteStatusIsFalse(id);
    public ActionPlan findByActionPlanCatalogIdAndPlanIdAndStrategyObjectTitleAndDeletedIsFalse(long? id, long? plan, string? title) =>
        CatalogAndPeriod(id, plan).SingleOrDefault(x => title != null && x.StrategyObject != null && x.StrategyObject.Title == title && x.DeleteStatus == false)!;
    public List<ActionPlan> findByPayeshgarIdAndPlanIdAndDeletedIsFalse(long? id, long? plan) =>
        Plans.Where(x => id != null && plan != null && x.Payeshgar != null && x.Payeshgar.UserId == id && x.PlanPeriod!.PlanId == plan && x.DeleteStatus == false).ToList();
    public List<ActionPlan> findByModirIdAndPlanIdAndDeletedIsFalse(long? id, long? plan) =>
        Plans.Where(x => id != null && plan != null && x.Modir != null && x.Modir.UserId == id && x.PlanPeriod!.PlanId == plan && x.DeleteStatus == false).ToList();
    public List<ActionPlan> findAllByStrategyObjectIdAndDeletedIsTrue(List<long?> ids) => Plans.Where(x => x.StrategyObject != null && ids.Contains(x.StrategyObject.Id) && x.DeleteStatus == true).ToList();
    public List<ActionPlan> findActionPlanByIdAndStrategyObjectAndActionPlanCatalog(long? id) =>
        Plans.Include(x => x.ActionPlanCatalog).Where(x => id != null && Plans.Any(a => a.Id == id &&
            a.StrategyObject != null && a.ActionPlanCatalog != null && x.StrategyObject != null && x.ActionPlanCatalog != null &&
            a.StrategyObject.Id == x.StrategyObject.Id && a.ActionPlanCatalog.Id == x.ActionPlanCatalog.Id)).ToList();
    public List<ActionPlan> findAllWithTaskUrlIsNotNull() => Plans.Where(x => x.TaskUrl != null && x.DeleteStatus == false).ToList();
    public bool existsByActionPlanCatalogIdAndLevelId(long? id, long? level) =>
        ByCatalog(id).Any(x => level != null && x.PlaningLevel != null && x.PlaningLevel.Id == level && x.DeleteStatus == false);
    public List<ActionPlan> findAllByActionPlanCatalogIds(HashSet<long?> ids) => Plans.Where(x => x.ActionPlanCatalog != null && ids.Contains(x.ActionPlanCatalog.Id) && x.DeleteStatus == false).ToList();
    public List<ActionPlan> findAllByCatalogIdAndDeletedFalse(long? id) => findByCatalogIdAndDeleteStatusIsFalse(id);
    public List<ActionPlan> findByActionPlanCatalog_IdOrderByPlanPeriod_PlanIdAsc(long? id) => ByCatalog(id).OrderBy(x => x.PlanPeriod!.PlanId).ToList();
    public ActionPlan findByActionPlanCatalog_IdAndPlanPeriod_PlanId(long? id, long? plan) => CatalogAndPeriod(id, plan).SingleOrDefault()!;
}
