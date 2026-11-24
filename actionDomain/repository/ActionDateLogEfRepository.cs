#nullable enable
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.baseDomain.repository;

namespace SemBackDotNet10.actionDomain.repository;

// EF implementation of the Spring repository in the supplied actionDomain.zip.
public sealed class ActionDateLogEfRepository(DbContext db)
    : SemRepository<ActionDateLog, long?>(db), ActionDateLogRepository
{
    public ActionDateLog? findFirstByActionPlanCatalogOrderByModifiedDateDesc(
        ActionPlanCatalog actionPlanCatalog)
    {
        var query = Set.Where(x => x.ActionPlanCatalog == null);
        if (actionPlanCatalog is not null)
        {
            var id = actionPlanCatalog.Id;
            query = Set.Where(x => x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == id);
        }
        return query.OrderByDescending(x => x.ModifiedDate).FirstOrDefault();
    }

    public ActionDateLog? findFirstByActionPlanCatalogIdOrderByModifiedDateAsc(long? actionPlanCatalogId)
        => Set.Where(x => x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == actionPlanCatalogId)
            .OrderBy(x => x.ModifiedDate).FirstOrDefault();

    public int countByActionPlanCatalogId(long? actionPlanCatalogId)
        => Set.Count(x => x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == actionPlanCatalogId);

    public ActionDateLog? findByActionPlanCatalogAndLastModified(long? actionPlanCatalogId)
    {
        var query = Set.Where(x => x.ActionPlanCatalog != null
            && x.ActionPlanCatalog.Id == actionPlanCatalogId);
        // Keep the MAX subquery and single-result semantics of Java's @Query.
        // In particular, NULL timestamps do not equal SQL MAX(NULL).
        return query.Where(x => x.ModifiedDate != null
            && x.ModifiedDate == query.Max(y => y.ModifiedDate)).SingleOrDefault();
    }
}
