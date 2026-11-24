using global::SemBackDotNet10.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Model.Entity;
using global::Common.model.dto;
using PageRequest = global::Common.utils.PageRequest;
using SemBackDotNet10.Common.utils;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.basicDomain.Repositories;

/// <summary>
/// ریپازیتوری پایه – معادل الگوی getRepository()/save()/delete() جاوا.
/// سرویس‌ها از طریق این پایه به CRUD دسترسی دارند.
/// </summary>
public interface IBaseRepository<T> where T : class
{
    IQueryable<T> Query { get; }

    Task<T?> FindByIdAsync(long id, CancellationToken ct = default);

    Task<T> AddAsync(T entity, CancellationToken ct = default);

    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

    Task UpdateAsync(T entity, CancellationToken ct = default);

    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

    Task DeleteAsync(T entity, CancellationToken ct = default);

    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

/// <summary>
/// پیاده‌سازی پایه روی EF Core.
/// </summary>
public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly SemDbContext Context;

    protected BaseRepository(SemDbContext context) => Context = context;

    public IQueryable<T> Query => Context.Set<T>().AsQueryable();

    public virtual async Task<T?> FindByIdAsync(long id, CancellationToken ct = default) =>
        await Context.Set<T>().FindAsync(new object[] { id }, ct);

    public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await Context.Set<T>().AddAsync(entity, ct);
        await Context.SaveChangesAsync(ct);
        return entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
    {
        await Context.Set<T>().AddRangeAsync(entities, ct);
        await Context.SaveChangesAsync(ct);
    }

    public virtual Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        Context.Set<T>().Update(entity);
        return Context.SaveChangesAsync(ct);
    }

    public virtual Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
    {
        Context.Set<T>().UpdateRange(entities);
        return Context.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        Context.Set<T>().Remove(entity);
        await Context.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
    {
        Context.Set<T>().RemoveRange(entities);
        await Context.SaveChangesAsync(ct);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        Context.SaveChangesAsync(ct);
}

// ---------- ریپازیتوری‌های اختصاصی ----------

public interface IDelegationRepository : IBaseRepository<Delegation>
{
    /// <summary>معادل findByFromUserAndToUserAndDelegationStartDate.</summary>
    Task<List<Delegation>> FindByFromUserAndToUserAsync(long fromUserId, long toUserId, CancellationToken ct = default);

    /// <summary>معادل findAllByToUserAndCurrentDate.</summary>
    Task<List<long>> FindActiveToUserIdsAsync(long userId, DateTime currentDate, CancellationToken ct = default);

    /// <summary>معادل findAllByDelegationEndDate (منقضی‌شده‌ها).</summary>
    Task<List<Delegation>> FindExpiredAsync(CancellationToken ct = default);
}

public class DelegationRepository : BaseRepository<Delegation>, IDelegationRepository
{
    public DelegationRepository(SemDbContext context) : base(context) { }

    public Task<List<Delegation>> FindByFromUserAndToUserAsync(long fromUserId, long toUserId, CancellationToken ct = default) =>
        Context.Delegations
            .Where(d => d.FromUserId == fromUserId && d.ToUserId == toUserId && d.DelegationStatus)
            .ToListAsync(ct);

    public Task<List<long>> FindActiveToUserIdsAsync(long userId, DateTime currentDate, CancellationToken ct = default) =>
        Context.Delegations
            .Where(d => d.ToUserId == userId
                        && d.DelegationStatus
                        && d.DelegationStartDate <= currentDate
                        && d.DelegationEndDate >= currentDate)
            .Where(d => d.FromUserId.HasValue)
            .Select(d => d.FromUserId!.Value)
            .ToListAsync(ct);

    public Task<List<Delegation>> FindExpiredAsync(CancellationToken ct = default) =>
        Context.Delegations
            .Where(d => d.DelegationStatus && d.DelegationEndDate < DateTime.Today)
            .ToListAsync(ct);
}

public interface IMeasureUnitRepository : IBaseRepository<MeasureUnit>
{
    Task<MeasureUnit?> FindByCodeAsync(string code, CancellationToken ct = default);

    Task<PagedResult<MeasureUnitDto>> FindAllMeasureUnitsAsync(string? filter, PageRequest pageRequest, CancellationToken ct = default);
}

public class MeasureUnitRepository : BaseRepository<MeasureUnit>, IMeasureUnitRepository
{
    public MeasureUnitRepository(SemDbContext context) : base(context) { }

    public Task<MeasureUnit?> FindByCodeAsync(string code, CancellationToken ct = default) =>
        Context.MeasureUnits.FirstOrDefaultAsync(m => m.Code == code, ct);

    public async Task<PagedResult<MeasureUnitDto>> FindAllMeasureUnitsAsync(string? filter, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Context.MeasureUnits
            .Where(m => filter == null || (m.DesMeasureF ?? string.Empty).Contains(filter) || (m.DesMeasureE ?? string.Empty).Contains(filter))
            .Select(m => new MeasureUnitDto
            {
                Id = m.Id,
                DesMeasureF = m.DesMeasureF,
                DesMeasureE = m.DesMeasureE
            });

        var total = await query.LongCountAsync(ct);
        var items = await global::Common.utils.PageUtils.Apply(query, pageRequest).ToListAsync(ct);
        return new PagedResult<MeasureUnitDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }
}
