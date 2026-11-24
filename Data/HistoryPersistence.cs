using System.Security.Claims;
using Common.model.entity;
using Common.repository;
using Common.service.interf;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.Data;

public sealed class HistoryAuditContext(SemDbContext db) : IHistoryAuditContext
{
    public HistoryAuthor GetCreator(History history)
    {
        if (history.CreatedById is not long id)
            throw new InvalidOperationException("History has no creator.");
        return db.Users.Where(user => user.UserId == id)
            .Select(user => new HistoryAuthor(user.UserId, user.NameF, user.FamilyF))
            .SingleOrDefault() ?? throw new InvalidOperationException($"History creator {id} was not found.");
    }
}

public abstract class AuditedPersistenceSession<T>(SemDbContext db, IHttpContextAccessor http)
    : IPersistenceSession<T> where T : class
{
    public IQueryable<T> Query => db.Set<T>();
    protected abstract long? Id(T entity);
    protected abstract void Stamp(T entity, bool created, DateTime timestamp, long? userId);

    public T Save(T entity)
    {
        var entry = db.Entry(entity);
        var created = entry.State == EntityState.Added ||
            (entry.State == EntityState.Detached && Id(entity) is null or 0);
        var principal = http.HttpContext?.User;
        var userId = principal?.Identity?.IsAuthenticated == true &&
            long.TryParse(principal.FindFirstValue("user_id"), out var parsed) ? parsed : (long?)null;
        Stamp(entity, created, DateTime.Now, userId);
        if (entry.State == EntityState.Detached)
        {
            if (created) db.Set<T>().Add(entity);
            else
            {
                db.Set<T>().Update(entity);
                entry.Property("CreatedDate").IsModified = false;
                entry.Property("CreatedById").IsModified = false;
            }
        }
        db.SaveChanges();
        return entity;
    }

    public void Delete(T entity)
    {
        db.Set<T>().Remove(entity);
        db.SaveChanges();
    }
}

public sealed class HistoryPersistenceSession(SemDbContext db, IHttpContextAccessor http)
    : AuditedPersistenceSession<History>(db, http)
{
    protected override long? Id(History entity) => entity.Id;
    protected override void Stamp(History entity, bool created, DateTime timestamp, long? userId)
    {
        if (created) { entity.CreatedDate = timestamp; entity.CreatedById = userId; }
        entity.ModifiedDate = timestamp;
        entity.ModifiedById = userId;
    }
}

public sealed class CommentPersistenceSession(SemDbContext db, IHttpContextAccessor http)
    : AuditedPersistenceSession<Comment>(db, http)
{
    protected override long? Id(Comment entity) => entity.Id;
    protected override void Stamp(Comment entity, bool created, DateTime timestamp, long? userId)
    {
        if (created) { entity.CreatedDate = timestamp; entity.CreatedById = userId; }
        entity.ModifiedDate = timestamp;
        entity.ModifiedById = userId;
    }
}
