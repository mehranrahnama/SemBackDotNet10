using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Base.repository;

namespace SemBackDotNet10.Base.repository;

/// <summary>
/// پیاده‌سازی عمومی SemRepository با EF Core.
/// معادل کلاس SimpleJpaRepository در Spring Data JPA.
/// از نوع TId برای متدهای GetByIdAsync / ExistsByIdAsync استفاده می‌کند.
/// </summary>
public class SemRepository<TEntity, TId> : ISemRepository<TEntity, TId>
    where TEntity : class
{
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public SemRepository(DbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity> GetReferenceByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        return await DbSet.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new KeyNotFoundException($"{typeof(TEntity).Name} with ID '{id}' was not found.");
    }

    // ─────────────────────────── معادل JpaRepository ───────────────────────────

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        if (id is null) return null;
        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default)
        => GetByIdAsync(id, cancellationToken);

    public virtual Task<List<TItem>> SaveAllAsync<TItem>(List<TItem> entities,
        CancellationToken cancellationToken = default) where TItem : TEntity
        => SaveAllAsync((IEnumerable<TItem>)entities, cancellationToken);

    public virtual async Task<List<TItem>> SaveAllAsync<TItem>(IEnumerable<TItem> entities,
        CancellationToken cancellationToken = default) where TItem : TEntity
    {
        ArgumentNullException.ThrowIfNull(entities);
        var items = entities.ToList();

        foreach (var item in items)
        {
            ArgumentNullException.ThrowIfNull(item);
            var entry = Context.Entry(item);
            if (entry.State != EntityState.Detached)
                continue;

            var id = GetIdValue(item);
            if (id is not null && await ExistsByIdAsync((TId)id, cancellationToken))
                DbSet.Update(item);
            else
                await DbSet.AddAsync(item, cancellationToken);
        }

        await Context.SaveChangesAsync(cancellationToken);
        return items;
    }

    public virtual Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => DbSet.AsNoTracking().ToListAsync(cancellationToken);

    public virtual Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual Task<List<TEntity>> FindAllAsync(CancellationToken cancellationToken = default)
        => DbSet.AsNoTracking().ToListAsync(cancellationToken);

    public virtual Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return DbSet.AsNoTracking().Where(specification).ToListAsync(cancellationToken);
    }

    public virtual async Task DeleteByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity is not null)
            await DeleteAsync(entity, cancellationToken);
    }

    public virtual async Task<(List<TEntity> Items, long Total)> FindAllPagedAsync(
        Expression<Func<TEntity, bool>>? specification, int page, int size,
        CancellationToken cancellationToken = default)
    {
        if (page < 0) throw new ArgumentOutOfRangeException(nameof(page));
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));

        var query = DbSet.AsNoTracking();
        if (specification is not null)
            query = query.Where(specification);

        var total = await query.LongCountAsync(cancellationToken);
        var items = await query.Skip(checked(page * size)).Take(size).ToListAsync(cancellationToken);
        return (items, total);
    }

    public virtual async Task<TItem> SaveAsync<TItem>(TItem entity, CancellationToken cancellationToken = default)
        where TItem : TEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entry = Context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            // اگر موجودیت شناسه دارد و در پایگاه داده موجود است، به‌روزرسانی می‌شود؛ وگرنه درج می‌شود.
            var id = GetIdValue(entity);
            if (id is not null && await ExistsByIdAsync((TId)id, cancellationToken))
                DbSet.Update(entity);
            else
                await DbSet.AddAsync(entity, cancellationToken);
        }

        await Context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        DbSet.Remove(entity);
        return Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<bool> ExistsByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        if (id is null) return false;
        return await DbSet.FindAsync(new object[] { id }, cancellationToken) is not null;
    }

    public virtual Task<long> CountAsync(CancellationToken cancellationToken = default)
        => DbSet.LongCountAsync(cancellationToken);

    // ─────────────────── معادل JpaSpecificationExecutor ───────────────────

    public virtual async Task<List<TEntity>> FindAllAsync(
        Expression<Func<TEntity, bool>>? specification,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool descending = false,
        int? page = null,
        int? size = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyQuery(DbSet.AsNoTracking(), specification, orderBy, descending, page, size);
        return await query.ToListAsync(cancellationToken);
    }

    public virtual Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>> specification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return DbSet.AsNoTracking().FirstOrDefaultAsync(specification, cancellationToken);
    }

    public virtual Task<long> CountAsync(Expression<Func<TEntity, bool>>? specification,
        CancellationToken cancellationToken = default)
        => specification is null
            ? DbSet.LongCountAsync(cancellationToken)
            : DbSet.LongCountAsync(specification, cancellationToken);

    // ─────────────────── معادل QueryByExampleExecutor ───────────────────

    /// <summary>
    /// معادل Query-by-Example در Spring Data:
    /// خواصِ مقداردهی‌شده (non-null) نمونه به شرط و مساوی (==) تبدیل و ترکیب AND می‌شوند.
    /// </summary>
    public virtual Task<List<TEntity>> FindByExampleAsync(TEntity example,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(example);

        var predicate = BuildExamplePredicate(example);
        return DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    // ─────────────────────────── توابع کمکی ───────────────────────────

    private static IQueryable<TEntity> ApplyQuery(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, bool>>? specification,
        Expression<Func<TEntity, object>>? orderBy,
        bool descending,
        int? page,
        int? size)
    {
        if (specification is not null)
            query = query.Where(specification);

        if (orderBy is not null)
            query = descending
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);

        if (page.HasValue && size.HasValue && size.Value > 0)
            query = query
                .Skip(page.Value * size.Value) // صفحه‌ها از صفر شروع می‌شوند (مانند Spring Pageable)
                .Take(size.Value);

        return query;
    }

    private Expression<Func<TEntity, bool>> BuildExamplePredicate(TEntity example)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        Expression? body = null;

        foreach (var property in typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            // شناسه و مجموعه‌ها (ناوبری‌ها) مطابق رفتار Spring از Example کنار گذاشته می‌شوند.
            if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) &&
                property.PropertyType != typeof(string))
                continue;

            var value = property.GetValue(example);
            if (value is null) continue;

            var member = Expression.Property(parameter, property);
            var constant = Expression.Constant(value, property.PropertyType);
            var equality = Expression.Equal(member, constant);

            body = body is null ? equality : Expression.AndAlso(body, equality);
        }

        // اگر هیچ خاصیتی مقدار نداشت، شرط همیشه‌درست (همه‌ی رکوردها) برمی‌گردد.
        body ??= Expression.Constant(true);

        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    private object? GetIdValue(TEntity entity)
    {
        var idProperty = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));

        return idProperty?.GetValue(entity);
    }
}
