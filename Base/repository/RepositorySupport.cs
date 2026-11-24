#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.baseDomain.repository;

public sealed record PageRequest(int PageNumber = 0, int PageSize = 20)
{
    public int Offset
    {
        get
        {
            if (PageNumber < 0) throw new ArgumentOutOfRangeException(nameof(PageNumber));
            if (PageSize <= 0) throw new ArgumentOutOfRangeException(nameof(PageSize));
            return checked(PageNumber * PageSize);
        }
    }
}

public sealed record PageResult<T>(IReadOnlyList<T> Content, long TotalElements, int PageNumber, int PageSize)
{
    public long TotalPages => PageSize <= 0
        ? throw new ArgumentOutOfRangeException(nameof(PageSize))
        : TotalElements / PageSize + (TotalElements % PageSize == 0 ? 0 : 1);
}

public interface ISemRepository<TEntity, TKey> where TEntity : class
{
    // Original API retained, including ValueTask.
    ValueTask<TEntity?> FindByIdAsync(TKey id, CancellationToken ct = default);
    Task<List<TEntity>> FindAllAsync(CancellationToken ct = default);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task<TEntity> GetReferenceByIdAsync(TKey id, CancellationToken ct = default);
    Task<S> SaveAsync<S>(S entity, CancellationToken ct = default) where S : TEntity;
    Task<List<S>> SaveAllAsync<S>(List<S> entities, CancellationToken ct = default) where S : TEntity;
    Task DeleteByIdAsync(TKey id, CancellationToken ct = default);
    Task DeleteAsync(TEntity entity, CancellationToken ct = default);
    Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    Task<(List<TEntity> Items, long Total)> FindAllPagedAsync(
        Expression<Func<TEntity, bool>>? predicate, int page, int size, CancellationToken ct = default);
}

public abstract class SemRepository<TEntity, TKey>(DbContext db)
    : ISemRepository<TEntity, TKey>, global::SemBackDotNet10.Base.repository.ISemRepository<TEntity, TKey>
    where TEntity : class
{
    protected DbContext Db { get; } = db;
    protected DbSet<TEntity> Set => Db.Set<TEntity>();

    public ValueTask<TEntity?> FindByIdAsync(TKey id, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return id is null ? ValueTask.FromResult<TEntity?>(null)
            : Set.FindAsync(new object?[] { id }, ct);
    }
    Task<TEntity?> global::SemBackDotNet10.Base.repository.ISemRepository<TEntity, TKey>.FindByIdAsync(
        TKey id, CancellationToken ct)
        => FindByIdAsync(id, ct).AsTask();

    public Task<List<TEntity>> FindAllAsync(CancellationToken ct = default) => Set.ToListAsync(ct);
    public void Add(TEntity entity) => Set.Add(entity);
    public void Update(TEntity entity) => Set.Update(entity);
    public void Remove(TEntity entity) => Set.Remove(entity);
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => Db.SaveChangesAsync(ct);

    // Unlike JPA getReferenceById, this loads the record and reports missing records immediately.
    public async Task<TEntity> GetReferenceByIdAsync(TKey id, CancellationToken ct = default)
        => await FindByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"{typeof(TEntity).Name} was not found.");

    // Track changes only. Caller owns SaveChangesAsync and any multi-step transaction.
    public Task<S> SaveAsync<S>(S entity, CancellationToken ct = default) where S : TEntity
    {
        ct.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(entity);
        if (Db.Entry(entity).State == EntityState.Detached)
        {
            if (Db.Entry(entity).IsKeySet) Db.Update(entity);
            else Db.Add(entity);
        }
        return Task.FromResult(entity);
    }
    public async Task<List<S>> SaveAllAsync<S>(List<S> entities, CancellationToken ct = default) where S : TEntity
    {
        ArgumentNullException.ThrowIfNull(entities);
        foreach (S entity in entities) await SaveAsync(entity, ct);
        return entities;
    }
    public async Task DeleteByIdAsync(TKey id, CancellationToken ct = default)
        => await DeleteAsync(await GetReferenceByIdAsync(id, ct), ct);
    public Task DeleteAsync(TEntity entity, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        Remove(entity);
        return Task.CompletedTask;
    }
    public Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        => Set.Where(predicate).ToListAsync(ct);

    public async Task<(List<TEntity> Items, long Total)> FindAllPagedAsync(
        Expression<Func<TEntity, bool>>? predicate, int page, int size, CancellationToken ct = default)
    {
        var request = new PageRequest(page, size);
        _ = request.Offset; // Validate before querying.
        IQueryable<TEntity> query = predicate is null ? Set : Set.Where(predicate);
        var result = await PageAsync(OrderForPaging(query), request, ct);
        return (result.Content.ToList(), result.TotalElements);
    }

    // Default stable ordering uses all mapped PK columns. Override for domain-specific ordering.
    protected virtual IOrderedQueryable<TEntity> OrderForPaging(IQueryable<TEntity> query)
    {
        var key = Db.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()
            ?? throw new InvalidOperationException("Paging requires an entity with a mapped primary key.");
        IOrderedQueryable<TEntity>? ordered = null;
        foreach (var property in key.Properties)
        {
            string name = property.Name;
            ordered = ordered is null
                ? query.OrderBy(x => EF.Property<object>(x, name))
                : ordered.ThenBy(x => EF.Property<object>(x, name));
        }
        return ordered!;
    }

    // Existing protected helper retained. The caller must supply deterministic ordering.
    protected static async Task<PageResult<T>> PageAsync<T>(
        IQueryable<T> orderedQuery, PageRequest page, CancellationToken ct)
    {
        int offset = page.Offset;
        long total = await orderedQuery.LongCountAsync(ct);
        var items = await orderedQuery.Skip(offset).Take(page.PageSize).ToListAsync(ct);
        return new(items, total, page.PageNumber, page.PageSize);
    }
}
