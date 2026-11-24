#nullable enable
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

/// <summary>Extracted catalog flag query, not a replacement for the catalog repository.</summary>
public sealed class ActionPlanCatalogPlanningReader(DbContext db)
{
    // Java bulk UPDATE: no deleted/status predicate and no implicit save or transaction.
    // ExecuteUpdate bypasses tracked entities; the caller owns tracker consistency.
    public void updatePlannedById(bool planned, long? id)
    {
        db.Set<ActionPlanCatalog>()
            .Where(x => id != null && x.Id == id.Value)
            .ExecuteUpdate(setters => setters.SetProperty(x => x.IsPlanned, planned));
    }

    // Java Optional<Long>: missing, deleted, or absent planning level produces null.
    public long? findCatalogLevelIdById(long? actionPlanCatalogId)
    {
        return db.Set<ActionPlanCatalog>()
            .Where(x => actionPlanCatalogId != null && x.Id == actionPlanCatalogId.Value
                && x.DeleteStatus == false && x.PlaningLevel != null)
            .Select(x => (long?)x.PlaningLevel!.Id)
            .SingleOrDefault();
    }

    // Java repository: SELECT apc.planned FROM ActionPlanCatalog apc WHERE apc.id = :id
    // No deleted predicate, default false, or inference from monthly plan rows.
    public bool findIsPlannedByActionPlanCatalogId(long? actionPlanCatalogId)
    {
        bool? planned = db.Set<ActionPlanCatalog>()
            .Where(x => actionPlanCatalogId != null && x.Id == actionPlanCatalogId.Value)
            .Select(x => (bool?)x.IsPlanned)
            .SingleOrDefault();
        return planned ?? throw new InvalidOperationException(
            "The catalog planning flag query returned no result; a Boolean value is required.");
    }
}
