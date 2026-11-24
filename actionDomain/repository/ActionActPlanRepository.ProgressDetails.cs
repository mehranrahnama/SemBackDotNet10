using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    public List<ActionProgressDto> FindProgressDetails(long? id, long? planId)
    {
        if (id is null || planId is null) return new();
        return (from act in Set.AsNoTracking()
                join ap in Db.Set<ActionPlan>().AsNoTracking()
                    on act.ActionPlanCatalog!.Id equals ap.ActionPlanCatalog!.Id
                where act.Id == id && ap.PlanPeriod != null && ap.PlanPeriod.PlanId == planId
                    && ap.DeleteStatus == false
                select new ActionProgressDto
                {
                    ActionActPlanId = act.Id,
                    ActionPlanCatalogDescription = act.ActionPlanCatalog!.Description,
                    ActionPlanCatalogId = act.ActionPlanCatalog.Id,
                    HadafName = ap.StrategyObject == null ? null : ap.StrategyObject.Title,
                    PayeshgarName = ap.Payeshgar == null ? null : ap.Payeshgar.NameF + " " + ap.Payeshgar.FamilyF,
                    PlanValue = act.PlanValue, ActualValue = act.ActualValue, ActualBudget = act.ActualBudget,
                    Deviation = act.Deviation, Correction = act.Correction, ActionActPlanConfirmStatus = act.Confirmstatus,
                    ModirId = ap.Modir == null ? null : ap.Modir.UserId,
                    PayeshgarId = ap.Payeshgar == null ? null : ap.Payeshgar.UserId,
                    LevelId = ap.PlaningLevel == null ? null : ap.PlaningLevel.Id,
                    Month = act.Month, TaskUrl = act.TaskUrl,
                    DesLevel = ap.PlaningLevel == null ? null : ap.PlaningLevel.DesLevel,
                    AddToSupremeLevel = ap.AddToSupremeLevel
                }).ToList();
    }

    public List<string> FindConnectedCommitteeNames(long catalogId)
    {
        var visited = FindConnectedCatalogIds(catalogId);
        return Db.Set<ActionPlanCatalog>().AsNoTracking()
            .Where(x => visited.Contains(x.Id) && x.PlaningLevel != null)
            .Select(x => x.PlaningLevel!.DesLevel).ToList();
    }

    public HashSet<long> FindConnectedCatalogIds(long catalogId)
    {
        var visited = new HashSet<long> { catalogId };
        var queue = new Queue<long>();
        queue.Enqueue(catalogId);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var edges = Db.Set<ActionPlanCatalogRelation>().AsNoTracking()
                .Where(x => x.MainActionPlanCatalog != null && x.RelatedActionPlanCatalog != null
                    && (x.MainActionPlanCatalog.Id == current || x.RelatedActionPlanCatalog.Id == current))
                .Select(x => new { Main = x.MainActionPlanCatalog!.Id, Related = x.RelatedActionPlanCatalog!.Id })
                .ToList();
            foreach (var edge in edges)
            {
                if (visited.Add(edge.Main)) queue.Enqueue(edge.Main);
                if (visited.Add(edge.Related)) queue.Enqueue(edge.Related);
            }
        }
        return visited;
    }

    public List<ActionActPlan> FindPlansForStatusPropagation(HashSet<long> catalogIds, long? month, long? planId)
        => month is null || planId is null ? new() : Set.Where(x => x.ActionPlanCatalog != null && catalogIds.Contains(x.ActionPlanCatalog.Id)
            && x.Month == month && x.PlanPeriod != null && x.PlanPeriod.PlanId == planId).ToList();
}
