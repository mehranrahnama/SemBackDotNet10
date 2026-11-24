#nullable enable
using System.Collections.Generic;
using System.Linq;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    // Java uses count(id) > 0. Any preserves that predicate without materializing rows.
    public bool existsByActionPlanCatalogIdAndDeletedIsFalse(long? actionPlanCatalogId)
        => actionPlanCatalogId is not null && Set.Any(x =>
            x.ActionPlanCatalog != null
            && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value
            && !x.IsDeleted);

    // No ordering in the source JPQL: consumers must not rely on row order.
    public List<long> findExpiredIdsByActionPlanCatalogId(long? actionPlanCatalogId)
        => actionPlanCatalogId is null ? new() : Set
            .Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value && x.IsDeleted)
            .Select(x => x.Id).ToList();

    // Keep the caller's tracking policy and global query filters intact.
    public List<ActionActPlan> findAllByActionPlanCatalogIdAndDeletedFalse(long? actionPlanCatalogId)
        => actionPlanCatalogId is null ? new() : Set
            .Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value && !x.IsDeleted)
            .ToList();
}
