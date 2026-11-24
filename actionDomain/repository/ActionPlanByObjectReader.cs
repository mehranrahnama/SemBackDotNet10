using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.Data;
using SemBackDotNet10.dashboardDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed class ActionPlanByObjectReader(SemDbContext db)
{
    public async Task<Dictionary<long, List<ActionPlanBeneficiaryDto>>> ReadBeneficiariesAsync(
        long tenantId, IReadOnlyCollection<long> actionIds, CancellationToken cancellationToken = default)
    {
        var ids = actionIds.Distinct().ToArray();
        var rows = await db.Set<ActionPlanBeneficiary>().IgnoreQueryFilters().AsNoTracking()
            .Where(item => item.ActionPlan != null && ids.Contains(item.ActionPlan.Id) &&
                item.ActionPlan.PlaningLevel != null && item.ActionPlan.PlaningLevel.FactoryId == tenantId)
            .Select(item => new
            {
                ActionId = item.ActionPlan!.Id,
                Data = new ActionPlanBeneficiaryDto
                {
                    ActionPlanBeneficiaryId = item.Id,
                    PlaningLevelOrganizationId = item.PlaningLevelOrganization != null ? item.PlaningLevelOrganization.Id : null,
                    DesOrganF = item.PlaningLevelOrganization != null && item.PlaningLevelOrganization.Organization != null
                        ? item.PlaningLevelOrganization.Organization.DesOrganF : null
                }
            }).ToListAsync(cancellationToken);
        return rows.GroupBy(row => row.ActionId).ToDictionary(group => group.Key, group => group.Select(row => row.Data).ToList());
    }

    public async Task<Dictionary<long, List<ActionPlanCoworkerDto>>> ReadCoworkersAsync(
        long tenantId, IReadOnlyCollection<long> actionIds, CancellationToken cancellationToken = default)
    {
        var ids = actionIds.Distinct().ToArray();
        var rows = await db.Set<ActionPlanCoworker>().IgnoreQueryFilters().AsNoTracking()
            .Where(item => item.ActionPlan != null && ids.Contains(item.ActionPlan.Id) &&
                item.ActionPlan.PlaningLevel != null && item.ActionPlan.PlaningLevel.FactoryId == tenantId)
            .Select(item => new
            {
                ActionId = item.ActionPlan!.Id,
                Data = new ActionPlanCoworkerDto
                {
                    ActionPlanCoworkerId = item.Id,
                    PlaningLevelOrganizationId = item.PlaningLevelOrganization != null ? item.PlaningLevelOrganization.Id : null,
                    DesOrganF = item.PlaningLevelOrganization != null && item.PlaningLevelOrganization.Organization != null
                        ? item.PlaningLevelOrganization.Organization.DesOrganF : null
                }
            }).ToListAsync(cancellationToken);
        return rows.GroupBy(row => row.ActionId).ToDictionary(group => group.Key, group => group.Select(row => row.Data).ToList());
    }

    public async Task<Dictionary<long, UpperLevelStrategyObjectDto>> ReadUpperStrategiesAsync(
        long tenantId, long activePlanId, IReadOnlyCollection<long> catalogIds, CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<long, UpperLevelStrategyObjectDto>();
        foreach (var catalogId in catalogIds.Distinct())
        {
            var visited = new HashSet<long> { catalogId };
            var current = catalogId;
            while (true)
            {
                var parentId = await db.Set<ActionPlanCatalogRelation>().AsNoTracking()
                    .Where(relation => relation.RelatedActionPlanCatalog != null &&
                        relation.RelatedActionPlanCatalog.Id == current && relation.RelatedActionPlanCatalog.PlaningLevel != null &&
                        relation.RelatedActionPlanCatalog.PlaningLevel.FactoryId == tenantId &&
                        relation.MainActionPlanCatalog != null && relation.MainActionPlanCatalog.PlaningLevel != null &&
                        relation.MainActionPlanCatalog.PlaningLevel.FactoryId == tenantId)
                    .Select(relation => (long?)relation.MainActionPlanCatalog!.Id)
                    .SingleOrDefaultAsync(cancellationToken);
                if (!parentId.HasValue || !visited.Add(parentId.Value))
                    break;
                current = parentId.Value;

                var strategy = await db.DomainActionPlans.IgnoreQueryFilters().AsNoTracking()
                    .Where(plan => plan.ActionPlanCatalog != null && plan.ActionPlanCatalog.Id == current &&
                        plan.ActionPlanCatalog.PlaningLevel != null && plan.ActionPlanCatalog.PlaningLevel.FactoryId == tenantId &&
                        plan.PlaningLevel != null && plan.PlaningLevel.FactoryId == tenantId && plan.PlaningLevel.TypeLevel == 1 &&
                        plan.PlanPeriod != null && plan.PlanPeriod.PlanId == activePlanId && plan.DeleteStatus == false &&
                        plan.StrategyObject != null && plan.StrategyObject.PlaningLevel != null &&
                        plan.StrategyObject.PlaningLevel.FactoryId == tenantId)
                    .OrderBy(plan => plan.Id)
                    .Select(plan => new UpperLevelStrategyObjectDto { Id = plan.StrategyObject!.Id, Title = plan.StrategyObject.Title })
                    .FirstOrDefaultAsync(cancellationToken);
                if (strategy is not null)
                {
                    result[catalogId] = strategy;
                    break;
                }
            }
        }
        return result;
    }

    public async Task<Dictionary<long, int>> ReadPlanningStatesAsync(long tenantId,
        IReadOnlyCollection<long> catalogIds, CancellationToken cancellationToken = default)
    {
        var ids = catalogIds.Distinct().ToArray();
        var states = await db.Set<ActionPlanCatalog>().IgnoreQueryFilters().AsNoTracking()
            .Where(catalog => ids.Contains(catalog.Id) && catalog.PlaningLevel != null &&
                              catalog.PlaningLevel.FactoryId == tenantId)
            .Select(catalog => new
            {
                catalog.Id,
                catalog.IsPlanned,
                HasPlans = db.ActionActPlans.IgnoreQueryFilters().Any(plan => plan.ActionPlanCatalog != null && plan.ActionPlanCatalog.Id == catalog.Id),
                HasNullValue = db.ActionActPlans.IgnoreQueryFilters().Any(plan => plan.ActionPlanCatalog != null && plan.ActionPlanCatalog.Id == catalog.Id && plan.PlanValue == null)
            }).ToListAsync(cancellationToken);
        return states.ToDictionary(state => state.Id,
            state => state.IsPlanned && state.HasPlans ? 2 : !state.IsPlanned && state.HasPlans && !state.HasNullValue ? 1 : 0);
    }

    public async Task<List<ActionPlanActivityDto>> ReadActivitiesAsync(long tenantId,
        IReadOnlyCollection<long> catalogIds, CancellationToken cancellationToken = default)
    {
        var ids = catalogIds.Distinct().ToArray();
        return await ActivityQuery(tenantId, ids).ToListAsync(cancellationToken);
    }

    public IQueryable<ActionPlanActivityDto> ActivityQuery(long tenantId, long[] ids) =>
        db.Set<ActionPlanActivity>().IgnoreQueryFilters().AsNoTracking()
            .Where(activity => activity.ActionPlanCatalog != null && ids.Contains(activity.ActionPlanCatalog.Id) &&
                activity.ActionPlanCatalog.PlaningLevel != null && activity.ActionPlanCatalog.PlaningLevel.FactoryId == tenantId)
            .Select(activity => new ActionPlanActivityDto
            {
                Id = activity.Id, ActivityName = activity.ActivityName, DateStart = activity.DateStart,
                DateEnd = activity.DateEnd, AccountableId = activity.Accountable != null ? activity.Accountable.UserId : null,
                AccountableName = activity.Accountable != null ? (activity.Accountable.NameF ?? "") + " " + (activity.Accountable.FamilyF ?? "") : " ",
                Weight = activity.Weight, EntityId = activity.EntityId, ActionPlanCatalogId = activity.ActionPlanCatalog!.Id
            });

    public IQueryable<ActionPlanDto> Query(long tenantId, long userId, long levelId,
        long planId, long objectId, long? status, string? filter)
    {
        var plans = db.DomainActionPlans.IgnoreQueryFilters().AsNoTracking()
            .Where(plan => plan.PlaningLevel != null && plan.PlaningLevel.FactoryId == tenantId &&
                plan.PlaningLevel.Id == levelId && plan.StrategyObject != null &&
                plan.StrategyObject.Id == objectId && plan.StrategyObject.PlaningLevel != null &&
                plan.StrategyObject.PlaningLevel.FactoryId == tenantId &&
                (plan.ActionPlanCatalog == null || (plan.ActionPlanCatalog.PlaningLevel != null &&
                    plan.ActionPlanCatalog.PlaningLevel.FactoryId == tenantId)) &&
                plan.PlanPeriod != null && plan.PlanPeriod.PlanId == planId &&
                plan.DeleteStatus == false && plan.Confirmstatus != null && plan.Confirmstatus != 50 &&
                (status == null || plan.Confirmstatus == status));

        if (filter is not null)
        {
            var pattern = $"%{filter}%";
            plans = plans.Where(plan => EF.Functions.Like(plan.Id.ToString(), pattern) ||
                EF.Functions.Like(plan.Priority.ToString(), pattern) ||
                (plan.Weight != null && EF.Functions.Like(plan.Weight.Value.ToString(), pattern)) ||
                EF.Functions.Like((plan.Modir != null ? (plan.Modir.NameF ?? "") + (plan.Modir.FamilyF ?? "") : "") +
                    (plan.Payeshgar != null ? (plan.Payeshgar.NameF ?? "") + (plan.Payeshgar.FamilyF ?? "") : "") +
                    (plan.ActionPlanCatalog != null ? plan.ActionPlanCatalog.Description : "") +
                    (plan.StrategyObject != null ? plan.StrategyObject.Title : ""), pattern));
        }

        return plans.Select(plan => new ActionPlanDto
        {
            Id = plan.Id,
            ActionPlanCatalogDescription = plan.ActionPlanCatalog != null ? plan.ActionPlanCatalog.Description : null,
            CommitteeCode = plan.PlaningLevel!.DesThreeChar,
            ActionPlanCatalogCode = plan.ActionPlanCatalog != null ? plan.ActionPlanCatalog.Code : null,
            Modir = plan.Modir != null ? (plan.Modir.NameF ?? "") + " " + (plan.Modir.FamilyF ?? "") : " ",
            ModirId = plan.Modir != null ? plan.Modir.UserId : null,
            Payeshgar = plan.Payeshgar != null ? (plan.Payeshgar.NameF ?? "") + " " + (plan.Payeshgar.FamilyF ?? "") : " ",
            PayeshgarId = plan.Payeshgar != null ? plan.Payeshgar.UserId : null,
            Priority = plan.Priority,
            Weight = plan.Weight,
            ActionPlanCatalogId = plan.ActionPlanCatalog != null ? plan.ActionPlanCatalog.Id : null,
            Type = plan.Type,
            StartDate = plan.ActionPlanCatalog != null ? plan.ActionPlanCatalog.StartDate : null,
            EndDate = plan.ActionPlanCatalog != null ? plan.ActionPlanCatalog.EndDate : null,
            Status = plan.Confirmstatus,
            YearStartPcn = plan.YearStartPcn,
            YearCompletePcn = plan.YearCompletePcn,
            AddedToDashboard = db.Set<Dashboard>().Any(item => !item.Deleted && item.UserId == userId && item.ActionPlanId == plan.Id),
            SendToUpperLevels = plan.AddToSupremeLevel,
            SendToParentOnly = plan.SendToParentOnly
        });
    }
}
