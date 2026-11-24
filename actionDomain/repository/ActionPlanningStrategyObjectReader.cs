#nullable enable
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.repository;

/// <summary>Strategy-object lookups used by action planning, preserving Java predicates.</summary>
public sealed class ActionPlanningStrategyObjectReader(DbContext db)
{
    // Java StrategyObjectImpl: resolve the active source title first, then query peers.
    // No access check, period restriction, deduplication, or ordering is added.
    public List<long> findStrategyObjectByTitleAndLevelId(long? strategyObjectId, long? levelId)
    {
        var title = findTitleById(strategyObjectId)
            ?? throw new BusinessException("هدف مورد نظر یافت نشد!");
        return findByTitleAndLevelId(title, levelId);
    }

    public string? findTitleById(long? strategyObjectId)
    {
        return db.Set<StrategyObject>()
            .Where(x => strategyObjectId != null && x.Id == strategyObjectId.Value
                && x.Deleted == false)
            .Select(x => x.Title)
            .SingleOrDefault();
    }

    public List<long> findByTitleAndLevelId(string? title, long? levelId)
    {
        // JPQL equality does not match null parameters; nullable deleted is not false.
        // Java specifies neither a plan-period predicate nor ordering here.
        return db.Set<StrategyObject>()
            .Where(x => title != null && levelId != null && x.Title == title
                && x.PlaningLevelId == levelId && x.Deleted == false)
            .Select(x => x.Id)
            .ToList();
    }
}
