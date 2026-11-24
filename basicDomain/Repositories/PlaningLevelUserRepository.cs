#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SemBackDotNet10.baseDomain.repository;

public interface IPlaningLevelUserRepository : ISemRepository<PlaningLevelUser, long>
{
    Task<List<long>> GetCurrentUserLevelIdsAsync(long userId, CancellationToken ct = default);
    Task<List<long>> FindLeveIdsByUserIdAsync(long userId, CancellationToken ct = default);
    Task<PlaningLevelUser?> FindByUserIdAndLevelIdAsync(long userId, long levelId, CancellationToken ct = default);
    Task<PlaningLevelUser?> FindByLevelIdAndUserIdAsync(long planingLevel, long userId, CancellationToken ct = default);
    Task<List<PlaningLevelUser>> FindByUserIdAsync(long userId, CancellationToken ct = default);
    Task<List<User>> FindUsersByLevelIdAsync(long levelId, CancellationToken ct = default);
    Task<bool> CheckIsUserInPlanningLevelAsync(long planningLevelId, long userId, CancellationToken ct = default);
    Task<bool> ExistsByLevelIdAndUserIdAsync(long levelId, long userId, CancellationToken ct = default);
}

public sealed class PlaningLevelUserRepository(DbContext db)
    : SemRepository<PlaningLevelUser, long>(db), IPlaningLevelUserRepository
{
    public Task<List<long>> GetCurrentUserLevelIdsAsync(long userId, CancellationToken ct = default)
    {
        return Set.Where(x => x.User.UserId == userId).Select(x => x.Level.Id).ToListAsync(ct);
    }

    public Task<List<long>> FindLeveIdsByUserIdAsync(long userId, CancellationToken ct = default)
    {
        return Set.Where(x => x.User.UserId == userId).Select(x => x.Level.Id).ToListAsync(ct);
    }

    public Task<PlaningLevelUser?> FindByUserIdAndLevelIdAsync(long userId, long levelId, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.User.UserId == userId && x.Level.Id == levelId, ct);
    }

    public Task<PlaningLevelUser?> FindByLevelIdAndUserIdAsync(long planingLevel, long userId, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.Level.Id == planingLevel && x.User.UserId == userId, ct);
    }

    public Task<List<PlaningLevelUser>> FindByUserIdAsync(long userId, CancellationToken ct = default)
    {
        return Set.Where(x => x.User.UserId == userId).ToListAsync(ct);
    }

    public Task<List<User>> FindUsersByLevelIdAsync(long levelId, CancellationToken ct = default)
    {
        return Set.Where(x => x.Level.Id == levelId).Select(x => x.User).ToListAsync(ct);
    }

    public Task<bool> CheckIsUserInPlanningLevelAsync(long planningLevelId, long userId, CancellationToken ct = default)
    {
        return Set.AnyAsync(x => x.Level.Id == planningLevelId && x.User.UserId == userId, ct);
    }

    public Task<bool> ExistsByLevelIdAndUserIdAsync(long levelId, long userId, CancellationToken ct = default)
    {
        return Set.AnyAsync(x => x.Level.Id == levelId && x.User.UserId == userId, ct);
    }
}
