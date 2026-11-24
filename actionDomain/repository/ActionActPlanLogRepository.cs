#nullable enable
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Base.repository;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed class ActionActPlanLogRepository(DbContext db)
    : ActionEfRepository<ActionActPlanLog, long>(db)
{
    protected override IOrderedQueryable<ActionActPlanLog> OrderForPaging(IQueryable<ActionActPlanLog> query)
        => query.OrderBy(x => x.Id);

    public List<ActionActPlanLog> findByActionPlanCatalogId(long? catalogId)
        => catalogId is null ? new List<ActionActPlanLog>() : Set
            .Where(x => x.ActionPlanCatalogId == catalogId).OrderBy(x => x.Id).ToList();

    // Sync methods preserve the supplied Java-shaped service contract without blocking async Tasks.
    public void SaveAllAndCommit(List<ActionActPlanLog> logs)
    {
        ArgumentNullException.ThrowIfNull(logs);
        foreach (var log in logs)
        {
            if (Db.Entry(log).State != EntityState.Detached) continue;
            if (Db.Entry(log).IsKeySet) Db.Update(log);
            else Db.Add(log);
        }
        Db.SaveChanges();
    }

    public void DeleteByCatalogAndCommit(long? catalogId)
    {
        if (catalogId is null) throw new ArgumentNullException(nameof(catalogId));
        Set.RemoveRange(findByActionPlanCatalogId(catalogId));
        Db.SaveChanges();
    }
}
