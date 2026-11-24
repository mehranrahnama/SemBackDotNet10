#nullable enable
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Base.repository;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed class ActionPlanCatalogEfRepository(SemDbContext db)
    : SemRepository<ActionPlanCatalog, long?>(db), ActionPlanCatalogRepository
{
    private IQueryable<ActionPlanCatalog> Catalogs => db.Set<ActionPlanCatalog>().IgnoreQueryFilters();

    public List<ActionPlanCatalog> findAllConnectedActions() =>
        Catalogs.Where(c => db.DomainActionPlans.IgnoreQueryFilters().Any(a =>
            a.ActionPlanCatalog != null && a.ActionPlanCatalog.Id == c.Id)).ToList();

    public ActionPlanCatalog findActionPlanCatalogByActionPlanId(long? actionPlanId) =>
        Catalogs.SingleOrDefault(c => actionPlanId != null && db.DomainActionPlans.IgnoreQueryFilters().Any(a =>
            a.Id == actionPlanId && a.ActionPlanCatalog != null && a.ActionPlanCatalog.Id == c.Id))!;

    public ActionPlanCatalogDto findActionPlanCatalogById(long? id) => Catalogs
        .Where(x => id != null && x.Id == id)
        .Select(x => new ActionPlanCatalogDto
        {
            Id = x.Id, Code = x.Code, Description = x.Description, StartDate = x.StartDate,
            EndDate = x.EndDate, TotalBudget = x.TotalBudget, ActionConfirmStatus = x.Confirmstatus,
            LevelId = x.PlaningLevel == null ? null : x.PlaningLevel.Id,
            DesLevel = x.PlaningLevel!.DesLevel, DeleteStatus = x.DeleteStatus,
            Payeshgar = (x.Payeshgar!.NameF ?? "") + " " + (x.Payeshgar.FamilyF ?? ""),
            Modir = (x.Modir!.NameF ?? "") + " " + (x.Modir.FamilyF ?? ""),
            ModirId = x.Modir == null ? null : x.Modir.UserId,
            PayeshgarId = x.Payeshgar == null ? null : x.Payeshgar.UserId
        }).SingleOrDefault()!;

    public List<ActionPlanCatalog> findByModirIdAndDeletedIsFalse(long? id) =>
        Catalogs.Where(x => id != null && x.Modir != null && x.Modir.UserId == id && x.DeleteStatus == false).ToList();

    public List<ActionPlanCatalog> findByPayeshgarIdAndDeletedIsFalse(long? id) =>
        Catalogs.Where(x => id != null && x.Payeshgar != null && x.Payeshgar.UserId == id && x.DeleteStatus == false).ToList();

    public long? findConfirmStatusById(long? id) =>
        Catalogs.Where(x => id != null && x.Id == id).Select(x => x.Confirmstatus).SingleOrDefault();

    public void resetPlanningFlagsByIds(Collection<long> ids)
    {
        ArgumentNullException.ThrowIfNull(ids);
        if (ids.Count == 0) return;
        var keys = ids.ToArray();
        Catalogs.Where(x => keys.Contains(x.Id)).ExecuteUpdate(setters => setters
            .SetProperty(x => x.IsPlanned, false)
            .SetProperty(x => x.IsReplanned, false)
            .SetProperty(x => x.ReplannedStatus, (long?)0));
    }

    public bool findIsPlannedByActionPlanCatalogId(long? id) =>
        Catalogs.Where(x => id != null && x.Id == id).Select(x => x.IsPlanned).Single();

    public List<ActionPlanCatalog> findAllWithTaskUrlIsNotNull() =>
        Catalogs.Where(x => x.TaskUrl != null && x.DeleteStatus == false).ToList();

    public List<ActionPlanCatalog> findPlannedCatalogsWithNoPlanningRecordsOrOnlyNullPlanningValues() =>
        Catalogs.Where(c => c.IsPlanned && !db.ActionActPlans.IgnoreQueryFilters()
            .Any(a => a.ActionPlanCatalog != null && a.ActionPlanCatalog.Id == c.Id &&
                      (a.PlanValue != null || a.ActualValue != null))).ToList();
}
