using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.basicDomain.Model.Entity;
using ActionPlan = SemBackDotNet10.actionDomain.model.entity.ActionPlan;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    public List<long> FindDirectSublevels(long levelId, long tenantId)
        => Db.Set<PlaningLevel>().AsNoTracking().Where(x => x.ParentId == levelId && x.FactoryId == tenantId)
            .Select(x => x.Id).ToList();

    public bool GridLevelExists(long levelId, long tenantId)
        => Db.Set<PlaningLevel>().AsNoTracking().Any(x => x.Id == levelId && x.FactoryId == tenantId);

    public string? FindGridLevelName(long levelId)
        => Db.Set<PlaningLevel>().AsNoTracking().Where(x => x.Id == levelId)
            .Select(x => x.DesLevel).SingleOrDefault();

    public List<ActionProgressDto> FindProgressGrid(string? filter, List<long> levels, long? planId,
        long? status, long? month, long? modirId, long userId, bool admin, bool sanaye, long tenantId)
    {
        var pattern = filter is null ? null : $"%{filter}%";
        var rows = from act in Set.AsNoTracking()
                   join ap in Db.Set<ActionPlan>().AsNoTracking()
                       on act.ActionPlanCatalog!.Id equals ap.ActionPlanCatalog!.Id
                   where !act.IsDeleted && act.PlanValue != null && ap.DeleteStatus == false
                       && act.ActionPlanCatalog!.PlaningLevel != null
                       && act.ActionPlanCatalog.PlaningLevel.FactoryId == tenantId
                       && ap.PlaningLevel != null && ap.PlaningLevel.FactoryId == tenantId
                       && levels.Contains(ap.PlaningLevel.Id)
                       && (ap.StrategyObject == null || (ap.StrategyObject.PlaningLevel != null
                           && ap.StrategyObject.PlaningLevel.FactoryId == tenantId))
                       && (planId == null || (ap.PlanPeriod != null && act.PlanPeriod != null
                           && ap.PlanPeriod.PlanId == planId && act.PlanPeriod.PlanId == planId))
                       && (month == null || act.Month == month) && (status == null || act.Confirmstatus == status)
                       && ((admin && (modirId == null || (ap.Modir != null && ap.Modir.UserId == modirId)))
                           || (ap.Modir != null && ap.Modir.UserId == userId && (status == 20 || status == null || (status != 0 && status != 20 && status != 30 && status != 70 && !(status == 40 && sanaye))))
                           || (ap.Payeshgar != null && ap.Payeshgar.UserId == userId && (status == 0 || status == null || (status != 0 && status != 20 && status != 30 && status != 70 && !(status == 40 && sanaye))))
                           || (ap.PlaningLevel.FacilitatorId == userId && (status == 30 || status == null || (status != 0 && status != 20 && status != 30 && status != 70 && !(status == 40 && sanaye))))
                           || (ap.PlaningLevel.ResponsibleId == userId && (status == 70 || status == null || (status != 0 && status != 20 && status != 30 && status != 70 && !(status == 40 && sanaye))))
                           || (status == 40 && sanaye))
                       && (pattern == null || (ap.StrategyObject != null && EF.Functions.Like(ap.StrategyObject.Title, pattern))
                           || EF.Functions.Like(act.ActionPlanCatalog!.Description, pattern)
                           || (ap.Payeshgar != null && EF.Functions.Like(ap.Payeshgar.NameF + " " + ap.Payeshgar.FamilyF, pattern))
                           || (ap.Modir != null && EF.Functions.Like(ap.Modir.NameF + " " + ap.Modir.FamilyF, pattern)))
                   select new { act, ap };
        // Java constructs a Pageable, but the repository uses its fixed ORDER BY
        // and the service slices the resulting list without re-sorting.
        var ordered = filter != null ? rows.OrderBy(x => x.act.Month)
            : month == null ? rows.OrderBy(x => x.act.ActionPlanCatalog!.Description)
            : rows.OrderBy(x => x.act.Id);
        return ordered.ThenBy(x => x.act.Id).ThenBy(x => x.ap.Id).Select(x => new ActionProgressDto
        {
            ActionActPlanId = x.act.Id, ActionPlanCatalogId = x.act.ActionPlanCatalog!.Id,
            ActionPlanCatalogDescription = x.act.ActionPlanCatalog.Description,
            HadafName = x.ap.StrategyObject == null ? null : x.ap.StrategyObject.Title,
            PayeshgarName = x.ap.Payeshgar == null ? null : x.ap.Payeshgar.NameF + " " + x.ap.Payeshgar.FamilyF,
            PlanValue = x.act.PlanValue, ActualValue = x.act.ActualValue, ActualBudget = x.act.ActualBudget,
            Deviation = x.act.Deviation, Correction = x.act.Correction, ActionActPlanConfirmStatus = x.act.Confirmstatus,
            ModirId = x.ap.Modir == null ? null : x.ap.Modir.UserId,
            ModirName = x.ap.Modir == null ? null : x.ap.Modir.NameF + " " + x.ap.Modir.FamilyF,
            PayeshgarId = x.ap.Payeshgar == null ? null : x.ap.Payeshgar.UserId,
            LevelId = x.ap.PlaningLevel!.Id, Month = x.act.Month,
            CorrectionStartDateAction = x.act.CorrectionStartDateAction, CorrectionDeadlineDate = x.act.CorrectionDeadlineDate,
            CorrectionExecuter = x.act.ActionPlanCatalog.Payeshgar == null ? null : x.act.ActionPlanCatalog.Payeshgar.UserId,
            ExecuterName = x.act.ActionPlanCatalog.Payeshgar == null ? null
                : x.act.ActionPlanCatalog.Payeshgar.NameF + " " + x.act.ActionPlanCatalog.Payeshgar.FamilyF,
            EndDate = x.act.ActionPlanCatalog.EndDate, StartDate = x.act.ActionPlanCatalog.StartDate,
            CorrectionActualValue = x.act.CorrectionActualValue, ActionPCN = x.act.ActionPCN,
            IsRoot = x.ap.PlaningLevel.ParentId == null && Db.Set<ActionPlanCatalogRelation>()
                .Any(r => r.MainActionPlanCatalog != null && r.MainActionPlanCatalog.Id == x.act.ActionPlanCatalog.Id)
        }).ToList();
    }

    public List<string> FindGridStrategyTitles(long catalogId, long planId, long tenantId)
        => Db.Set<ActionPlan>().AsNoTracking().Where(x => x.ActionPlanCatalog != null
            && x.ActionPlanCatalog.Id == catalogId && x.PlanPeriod != null && x.PlanPeriod.PlanId == planId
            && x.PlaningLevel != null && x.PlaningLevel.FactoryId == tenantId
            && x.DeleteStatus == false && x.StrategyObject != null
            && x.StrategyObject.PlaningLevel != null && x.StrategyObject.PlaningLevel.FactoryId == tenantId)
            .Select(x => x.StrategyObject!.Title).ToList();
}
