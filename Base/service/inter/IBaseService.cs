#nullable enable
using System.Linq.Expressions;
using SemBackDotNet10.Base.repository;

namespace SemBackDotNet10.Base.Service.Inter;

public interface IBaseService<T, TId, R>
    where T : class
    where R : ISemRepository<T, TId>
{
    R GetRepository();
    Task<T?> FindByIdAsync(TId id, CancellationToken ct = default)
        => GetRepository().FindByIdAsync(id, ct);
    Task<T> GetByIdAsync(TId id, CancellationToken ct = default)
        => GetRepository().GetReferenceByIdAsync(id, ct);
    Task<S> SaveAsync<S>(S entity, CancellationToken ct = default) where S : T
        => GetRepository().SaveAsync(entity, ct);
    Task<List<S>> SaveAllAsync<S>(List<S> entities, CancellationToken ct = default) where S : T
        => GetRepository().SaveAllAsync(entities, ct);
    Task DeleteByIdAsync(TId id, CancellationToken ct = default)
        => GetRepository().DeleteByIdAsync(id, ct);
    Task DeleteAsync(T entity, CancellationToken ct = default)
        => GetRepository().DeleteAsync(entity, ct);
    Task<List<T>> FindAllAsync(CancellationToken ct = default)
        => GetRepository().FindAllAsync(ct);
    Task<List<T>> FindAllAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => GetRepository().FindAllAsync(predicate, ct);
    Task<(List<T> Items, long Total)> FindAllPagedAsync(int page, int size, CancellationToken ct = default)
        => GetRepository().FindAllPagedAsync(null, page, size, ct);
    Task<(List<T> Items, long Total)> FindAllPagedAsync(
        Expression<Func<T, bool>> predicate, int page, int size, CancellationToken ct = default)
        => GetRepository().FindAllPagedAsync(predicate, page, size, ct);
}
