using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.measureDomain.model.entity;

namespace SemBackDotNet10.measureDomain.repository;

// The caller must authorize the requested level and derive the user, roles and allowed
// level list from the authenticated request before invoking this query.
internal static class MeasureProgressGridQuery
{
    internal static IOrderedQueryable<MeasureActPlan> Apply(IQueryable<MeasureActPlan> source,
        long[] allowedLevels, long planId, long? month, long? status, long? managerId,
        string? filter, long userId, bool admin, bool sanaye, bool mscHolding)
    {
        var industry = status == 40 && sanaye;
        var anyAssignment = status != 0 && status != 20 && status != 30 && status != 70 && !industry;
        var monitor = anyAssignment || status == 0;
        var manager = anyAssignment || status == 20;
        var facilitator = anyAssignment || status == 30;
        var responsible = anyAssignment || status == 70;
        var pattern = filter == null ? null : $"%{filter}%";

        var rows = from act in source.AsNoTracking()
                   let measure = act.Measure
                   let catalog = measure.MeasureCatalog
                   let level = catalog.PlaningLevel
                   let ownerId = mscHolding ? (long?)measure.Modir.UserId : catalog.Modir.UserId
                   let ownerName = mscHolding ? measure.Modir.NameF : catalog.Modir.NameF
                   let ownerFamily = mscHolding ? measure.Modir.FamilyF : catalog.Modir.FamilyF
                   let monitorId = mscHolding ? (long?)measure.Tashilgar.UserId : catalog.Tashilgar.UserId
                   where act.Active && !act.Deleted && act.PlanValue != null && measure.Planned
                       && measure.PlanPeriod.PlanId == planId && allowedLevels.Contains(level.Id)
                       && (!mscHolding || measure.Confirmstatus == 20 || level.ParentId == null)
                       && (month == null || act.Month == month)
                       && (status == null || act.Confirmstatus == status)
                       && ((admin && (managerId == null || ownerId == managerId))
                           || (manager && ownerId == userId)
                           || (monitor && monitorId == userId)
                           || (monitor && !mscHolding && catalog.TashilgarHamkar != null && catalog.TashilgarHamkar.UserId == userId)
                           || (facilitator && level.FacilitatorId == userId)
                           || (responsible && level.ResponsibleId == userId)
                           || industry)
                       && (pattern == null || EF.Functions.Like(catalog.MeasureRepository.MeasureName!, pattern)
                           || EF.Functions.Like(ownerName + " " + ownerFamily, pattern))
                   select act;
        return rows.OrderBy(act => act.Measure.MeasureCatalog.MeasureRepository.MeasureName)
            .ThenBy(act => act.Month).ThenBy(act => act.Id);
    }
}
