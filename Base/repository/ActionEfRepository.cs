#nullable enable
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.Base.repository;

public abstract class ActionEfRepository<T, TId>(DbContext db) : ISemRepository<T, TId>
    where T : class
{
    protected DbContext Db { get; } = db;
    protected DbSet<T> Set => Db.Set<T>();
    protected abstract IOrderedQueryable<T> OrderForPaging(IQueryable<T> query);

    public async Task<T?> FindByIdAsync(TId id, CancellationToken ct = default)
        => id is null ? null : await Set.FindAsync(new object?[] { id }, ct);
    public async Task<T> GetReferenceByIdAsync(TId id, CancellationToken ct = default)
        => await FindByIdAsync(id, ct) ?? throw new KeyNotFoundException($"{typeof(T).Name} was not found.");

    // SaveChanges is deliberately explicit. Use one shared scoped DbContext/transaction.
    public Task<S> SaveAsync<S>(S entity, CancellationToken ct = default) where S : T
    {
        ct.ThrowIfCancellationRequested();
        if (Db.Entry(entity).State == EntityState.Detached)
        {
            if (Db.Entry(entity).IsKeySet) Db.Update(entity);
            else Db.Add(entity);
        }
        return Task.FromResult(entity);
    }
    public async Task<List<S>> SaveAllAsync<S>(List<S> entities, CancellationToken ct = default) where S : T
    {
        ArgumentNullException.ThrowIfNull(entities);
        foreach (S entity in entities) await SaveAsync(entity, ct);
        return entities;
    }
    public async Task DeleteByIdAsync(TId id, CancellationToken ct = default)
        => await DeleteAsync(await GetReferenceByIdAsync(id, ct), ct);
    public Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        Set.Remove(entity);
        return Task.CompletedTask;
    }
    public Task<List<T>> FindAllAsync(CancellationToken ct = default) => Set.ToListAsync(ct);
    public Task<List<T>> FindAllAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => Set.Where(predicate).ToListAsync(ct);
    public async Task<(List<T> Items, long Total)> FindAllPagedAsync(
        Expression<Func<T, bool>>? predicate, int page, int size, CancellationToken ct = default)
    {
        if (page < 0) throw new ArgumentOutOfRangeException(nameof(page));
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
        int offset = checked(page * size);
        IQueryable<T> query = predicate is null ? Set : Set.Where(predicate);
        long total = await query.LongCountAsync(ct);
        List<T> items = await OrderForPaging(query).Skip(offset).Take(size).ToListAsync(ct);
        return (items, total);
    }
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => Db.SaveChangesAsync(ct);
}
