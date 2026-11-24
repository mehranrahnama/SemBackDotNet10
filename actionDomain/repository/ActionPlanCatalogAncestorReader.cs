#nullable enable
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed class ActionPlanCatalogAncestorReader(DbContext db)
{
    public long? findParentId(long? childId)
        => db.Set<ActionPlanCatalogRelation>()
            .Where(x => childId != null && x.RelatedActionPlanCatalog != null
                && x.RelatedActionPlanCatalog.Id == childId.Value
                && x.MainActionPlanCatalog != null)
            .Select(x => (long?)x.MainActionPlanCatalog!.Id)
            .SingleOrDefault();

    public ActionPlanCatalog? findParentByChildId(long? childId)
        => db.Set<ActionPlanCatalogRelation>()
            .Where(x => childId != null && x.RelatedActionPlanCatalog != null
                && x.RelatedActionPlanCatalog.Id == childId.Value
                && x.MainActionPlanCatalog != null)
            .Select(x => x.MainActionPlanCatalog!)
            .Include(x => x.PlaningLevel)
            .AsTracking()
            .SingleOrDefault();
}
