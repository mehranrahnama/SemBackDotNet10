using Common.repository;
using global::Common.model.entity;
using SemBackDotNet10.Base.repository;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

//using SemBackDotNet10.Base.Model.Entity;
//using SemBackDotNet10.Base.repository;
//using SemBackDotNet10.Common.Model.Entity;
//using SemBackDotNet10.Common.Repository;
//using SemBackDotNet10.Common.Service.Inter;
//using SemBackDotNet10.Common.Service.Impl;

namespace SemBackDotNet10.Base.Service.Inter;

/// <summary>
/// معادل ProcessService.java — default method های جاوا با DIM معادل شده‌اند.
/// @Transactional جاوا با SaveChanges/TransactionScope در EF Core پوشش داده می‌شود.
/// save بدون History و lockComment رد شود Exception جاوا → InvalidOperationException.
/// Process metadata is exposed through IProcessEntity, independent of the entity ID generic type.
/// </summary>
public interface IProcessService<T, TId, R>
    where T : class
    where R : ISemRepository<T, TId>
{
    CommentRepository GetCommentRepository();

    HistoryRepository GetHistoryRepository();

    R GetRepository();

    // ===== default methods =====
    Task<T?> FindByIdAsync(TId id, CancellationToken ct = default)
        => GetRepository().FindByIdAsync(id, ct);

    Task<T> GetByIdAsync(TId id, CancellationToken ct = default)
        => GetRepository().GetReferenceByIdAsync(id, ct);

    /// <summary>معادل save(S) با @Transactional — save + saveToHistory + lockComment</summary>
    async Task<S> SaveAsync<S>(S entity, CancellationToken ct = default) where S : T
    {
        S result = await GetRepository().SaveAsync(entity, ct);
        S? saveToHistory = await SaveToHistoryAsync(result, ct);
        if (saveToHistory is null)
            throw new InvalidOperationException("saveToHistory failed");
        bool locked = await LockCommentAsync(result, ct);
        if (!locked)
            throw new InvalidOperationException("lockComment failed");
        return result;
    }

    /// <summary>معادل saveWithoutHistory(S)</summary>
    Task<S> SaveWithoutHistoryAsync<S>(S entity, CancellationToken ct = default) where S : T
        => GetRepository().SaveAsync(entity, ct);

    /// <summary>معادل lockComment(S) با @Transactional</summary>
    Task<bool> LockCommentAsync<S>(S result, CancellationToken ct = default) where S : T
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var process = (IProcessEntity)result;
            var repository = GetCommentRepository();
            var comments = repository.FindAllByEntityIdAndEntityName(
                process.ProcessId ?? throw new InvalidOperationException("The process entity has no ID."),
                result.GetType().Name);
            foreach (var comment in comments)
            {
                ct.ThrowIfCancellationRequested();
                comment.Lock = true;
                repository.Save(comment);
            }
            return Task.FromResult(true);
        }
        catch (Exception) when (!ct.IsCancellationRequested)
        {
            Console.Error.WriteLine("lockComment failed");
            return Task.FromResult(false);
        }
    }

    /// <summary>معادل saveToHistory(S) — ساخت History و ذخیره از طریق ریپازیتوری هیستوری</summary>
    Task<S?> SaveToHistoryAsync<S>(S result, CancellationToken ct = default) where S : T
    {
        ct.ThrowIfCancellationRequested();
        History his = new History();
        try
        {
            var process = (IProcessEntity)result;
            his.EntityName = result.GetType().Name;
            his.EntityId = process.ProcessId ?? throw new InvalidOperationException("The process entity has no ID.");
            his.Status = process.ReplannedForHistory ? process.ReplanStatusForHistory : process.Confirmstatus;
            his.OutCome = process.OutCome;
            his.DeleteFirstStatus =
                (process.Confirmstatus == 30L || process.ReplannedForHistory)
                    ? process.DeleteFirstStatus
                    : 0L;
            ct.ThrowIfCancellationRequested();
            GetHistoryRepository().Save(his);
            return Task.FromResult<S?>(result);
        }
        catch (Exception) when (!ct.IsCancellationRequested)
        {
            Console.Error.WriteLine("saveToHistory failed");
            return Task.FromResult<S?>(default);
        }
    }

    Task<List<S>> SaveAllAsync<S>(List<S> entityList, CancellationToken ct = default) where S : T
        => GetRepository().SaveAllAsync(entityList, ct);

    Task DeleteByIdAsync(TId id, CancellationToken ct = default)
        => GetRepository().DeleteByIdAsync(id, ct);

    Task DeleteAsync(T entity, CancellationToken ct = default)
        => GetRepository().DeleteAsync(entity, ct);

    Task<List<T>> FindAllAsync(CancellationToken ct = default)
        => GetRepository().FindAllAsync(ct);

    Task<List<T>> FindAllAsync(Expression<Func<T, bool>> spec, CancellationToken ct = default)
        => GetRepository().FindAllAsync(spec, ct);

    Task<(List<T> Items, long Total)> FindAllPagedAsync(int page, int size, CancellationToken ct = default)
        => GetRepository().FindAllPagedAsync(null, page, size, ct);

    Task<(List<T> Items, long Total)> FindAllPagedAsync(
        Expression<Func<T, bool>> spec, int page, int size, CancellationToken ct = default)
        => GetRepository().FindAllPagedAsync(spec, page, size, ct);

    /// <summary>Equivalent of the Java findExpireTasks default method.</summary>
    Task<List<S>> FindExpireTasksAsync<S>(List<S> entities, string maxTakDate, CancellationToken ct = default) where S : T
    {
        List<S> expired = new List<S>();
        DateTime now = DateTime.Now;
        foreach (S task in entities)
        {
            var modifieddate = ((IProcessEntity)task).Modifieddate;
            if (modifieddate is not null)
            {
                DateTime expireDate = modifieddate.Value.AddDays(long.Parse(maxTakDate));
                if (expireDate < now)
                    expired.Add(task);
            }
        }
        return Task.FromResult(expired);
    }

}
