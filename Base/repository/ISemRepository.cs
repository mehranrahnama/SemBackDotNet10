#nullable enable
using System.Linq.Expressions;

namespace SemBackDotNet10.Base.repository;

// Add only if this exact namespace/type does not already exist in the Solution.
public interface ISemRepository<T, TId> where T : class
{
    Task<T?> FindByIdAsync(TId id, CancellationToken ct = default);
    Task<T> GetReferenceByIdAsync(TId id, CancellationToken ct = default);
    Task<S> SaveAsync<S>(S entity, CancellationToken ct = default) where S : T;
    Task<List<S>> SaveAllAsync<S>(List<S> entities, CancellationToken ct = default) where S : T;
    Task DeleteByIdAsync(TId id, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
    Task<List<T>> FindAllAsync(CancellationToken ct = default);
    Task<List<T>> FindAllAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<(List<T> Items, long Total)> FindAllPagedAsync(
        Expression<Func<T, bool>>? predicate, int page, int size, CancellationToken ct = default);
}
