#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Common.exception;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Common.enums;
namespace SemBackDotNet10.service.impl;


public sealed record ServicePageRequest(int Page, int Size, string OrderBy = "Id", SortDirection Direction = SortDirection.Ascending)
{
    public int Offset => Page < 0 || Size <= 0 ? throw new ArgumentOutOfRangeException(nameof(Page)) : checked(Page * Size);
}
public sealed record ServicePage<T>(IReadOnlyList<T> Content, long TotalElements, int CurrentPage, int PageSize)
{
    public long TotalPages => TotalElements / PageSize + (TotalElements % PageSize == 0 ? 0 : 1);
}

// All implementations must share this scoped DbContext. No sync-over-async adapters.
public sealed class ServiceRuntime(global::SemBackDotNet10.Data.SemDbContext db, IServiceQueries queries, IExternalDomainServices external,
    ICurrentUser currentUser, TimeProvider clock, IConfiguration configuration)
{
    public DbContext Db { get; } = db;
    public IServiceQueries Queries { get; } = queries;
    public IExternalDomainServices External { get; } = external;
    public ICurrentUser User { get; } = currentUser;
    public TimeProvider Clock { get; } = clock;
    public string TenantName { get; } = configuration["Security:DefaultTenantName"] is { Length: > 0 } tenantName
        ? tenantName : throw new InvalidOperationException("Security:DefaultTenantName is required.");
    public DateTime TehranToday => TimeZoneInfo.ConvertTime(Clock.GetUtcNow(), TimeZoneInfo.FindSystemTimeZoneById("Asia/Tehran")).Date;

    public async Task<T> TransactionAsync<T>(Func<Task<T>> action, CancellationToken ct)
    {
        if (Db.Database.CurrentTransaction is not null) return await action();
        var strategy = Db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Db.Database.BeginTransactionAsync(ct);
            try { var result = await action(); await Db.SaveChangesAsync(ct); await transaction.CommitAsync(ct); return result; }
            catch { await transaction.RollbackAsync(CancellationToken.None); throw; }
        });
    }
    public Task TransactionAsync(Func<Task> action, CancellationToken ct) => TransactionAsync(async () => { await action(); return true; }, ct);
    public async Task<T> RequireAsync<T>(long? id, string error, CancellationToken ct) where T : class
        => id is null ? throw new BusinessException(error) : await Db.Set<T>().FindAsync([id.Value], ct) ?? throw new BusinessException(error);
    public async Task<T> SaveAsync<T>(T entity, CancellationToken ct) where T : class
    {
        if (Db.Entry(entity).State == EntityState.Detached) Db.Set<T>().Add(entity);
        await Db.SaveChangesAsync(ct);
        return entity;
    }
}

public interface ICurrentUser
{
    long Id { get; }
    bool IsAdmin { get; } // exact Administrators role, from authenticated identity
}

public abstract class ServiceBase<TEntity>(ServiceRuntime runtime) where TEntity : class
{
    protected ServiceRuntime R { get; } = runtime;
    public DbSet<TEntity> GetRepository() => R.Db.Set<TEntity>();
    public async Task<TEntity?> FindByIdAsync(long id, CancellationToken ct = default) => await GetRepository().FindAsync([id], ct);
    public Task<TEntity> GetByIdAsync(long id, CancellationToken ct = default) => R.RequireAsync<TEntity>(id, "error.entity.idIsNotValid", ct);
    public Task<TEntity> SaveAsync(TEntity entity, CancellationToken ct = default) => R.SaveAsync(entity, ct);
    public async Task<List<TEntity>> SaveAllAsync(List<TEntity> entities, CancellationToken ct = default)
    {
        foreach (var entity in entities) if (R.Db.Entry(entity).State == EntityState.Detached) GetRepository().Add(entity);
        await R.Db.SaveChangesAsync(ct); return entities;
    }
    public async Task DeleteAsync(TEntity entity, CancellationToken ct = default)
    { GetRepository().Remove(entity); await R.Db.SaveChangesAsync(ct); }
    public async Task DeleteByIdAsync(long id, CancellationToken ct = default) => await DeleteAsync(await GetByIdAsync(id, ct), ct);
    public Task<List<TEntity>> FindAllAsync(CancellationToken ct = default) => GetRepository().ToListAsync(ct);
    // Java Specification / Example are replaced by an EF-translatable predicate, not in-memory evaluation.
    public Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        => GetRepository().Where(predicate).ToListAsync(ct);
    public Task<ServicePage<TEntity>> FindAllAsync(ServicePageRequest page, CancellationToken ct = default)
        => PageAsync(GetRepository(), page, ct);
    public Task<ServicePage<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, ServicePageRequest page, CancellationToken ct = default)
        => PageAsync(GetRepository().Where(predicate), page, ct);
    protected static async Task<ServicePage<T>> PageAsync<T>(IQueryable<T> query, ServicePageRequest page, CancellationToken ct)
    {
        var offset = page.Offset;
        // Validate member names before constructing an expression; never interpolate into SQL.
        var property = typeof(T).GetProperty(page.OrderBy, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase)
            ?? throw new ArgumentException("Unknown sort property", nameof(page));
        var p = Expression.Parameter(typeof(T), "x");
        var key = Expression.Lambda(Expression.Property(p, property), p);
        var call = Expression.Call(typeof(Queryable), page.Direction == SortDirection.Ascending ? "OrderBy" : "OrderByDescending", [typeof(T), property.PropertyType], query.Expression, Expression.Quote(key));
        var ordered = query.Provider.CreateQuery<T>(call);
        return new(await ordered.Skip(offset).Take(page.Size).ToListAsync(ct), await query.LongCountAsync(ct), page.Page, page.Size);
    }
    protected Task AccessAsync(long? levelId, CancellationToken ct) => new PlaningLevelUserServiceImpl(R).HasAccessInLevelIdAsync(levelId, R.User.Id, ct);
    protected Task<T> RequireAsync<T>(long? id, string error, CancellationToken ct) where T : class => R.RequireAsync<T>(id, error, ct);
}
