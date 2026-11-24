using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Repository;

/// <summary>نتیجهٔ صفحه‌بندی‌شده؛ معادل Page&lt;T&gt; اسپرینگ.</summary>
public sealed class PageResult<T>
{
    public List<T> Content { get; init; } = [];
    public long TotalElements { get; init; }
    public int TotalPages { get; init; }
}

/// <summary>پایهٔ ریپازیتوری‌های امنیتی؛ معادل SemRepository&lt;T, Long&gt; (متدهای همگام و ناهمگام).</summary>
public abstract class SecurityRepositoryBase<T> where T : class
{
    protected SecurityRepositoryBase(DbContext context)
    {
        Context = context;
    }

    protected DbContext Context { get; }

    protected DbSet<T> Set => Context.Set<T>();

    public T Save(T entity)
    {
        Attach(entity);
        Context.SaveChanges();
        return entity;
    }

    public List<T> SaveAll(IEnumerable<T> entities)
    {
        var list = entities.ToList();
        foreach (var entity in list)
            Attach(entity);
        Context.SaveChanges();
        return list;
    }

    public void Delete(T entity)
    {
        Set.Remove(entity);
        Context.SaveChanges();
    }

    public void DeleteAll(IEnumerable<T> entities)
    {
        Set.RemoveRange(entities);
        Context.SaveChanges();
    }

    public T? FindById(long id) => Set.Find(id);

    public List<T> FindAll() => Set.ToList();

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await Set.AddAsync(entity, ct);
        await Context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<T> SaveAsync(T entity, CancellationToken ct = default)
    {
        Attach(entity);
        await Context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        Set.Remove(entity);
        await Context.SaveChangesAsync(ct);
    }

    public async Task<T?> FindByIdAsync(long id, CancellationToken ct = default) =>
        await Set.FindAsync([id], ct);

    public Task<List<T>> FindAllAsync(CancellationToken ct = default) => Set.ToListAsync(ct);

    private void Attach(T entity)
    {
        var entry = Context.Entry(entity);
        if (entry.State != EntityState.Detached)
            return;

        if (entry.IsKeySet)
            Set.Update(entity);
        else
            Set.Add(entity);
    }

    protected static PageResult<TItem> ToPage<TItem>(List<TItem> content, long total, int size) => new()
    {
        Content = content,
        TotalElements = total,
        TotalPages = size > 0 ? (int)Math.Ceiling(total / (double)size) : 1
    };
}
