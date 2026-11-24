using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    public List<ActionPlanActivityDto> FindGridActivities(List<long> catalogIds)
        => Db.Set<ActionPlanActivity>().AsNoTracking()
            .Where(x => x.ActionPlanCatalog != null && catalogIds.Contains(x.ActionPlanCatalog.Id))
            .Select(x => new ActionPlanActivityDto
            {
                Id = x.Id, ActivityName = x.ActivityName, DateStart = x.DateStart, DateEnd = x.DateEnd,
                AccountableId = x.Accountable == null ? null : x.Accountable.UserId,
                AccountableName = x.Accountable == null ? null : x.Accountable.NameF + " " + x.Accountable.FamilyF,
                Weight = x.Weight, EntityId = x.EntityId, ActionPlanCatalogId = x.ActionPlanCatalog!.Id
            }).ToList();
}
