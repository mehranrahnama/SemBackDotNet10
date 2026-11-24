#nullable enable
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.service.impl;

public sealed class PlaningLevelUserServiceImpl(ServiceRuntime runtime) : ServiceBase<PlaningLevelUser>(runtime)
{
    public Task<List<long>> GetUserLevelIdsAsync(long userId, CancellationToken ct = default)
        => R.External.GetUserLevelIdsAsync("PlaningLevelUser", userId, ct);

    public Task CreateOrDeleteAsync(long userId, List<long> levelIds, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(levelIds);
            var user = await R.External.GetUserAsync(userId, ct);
            var previous = await GetRepository().Include(x => x.Level).Where(x => x.UserId == userId).ToListAsync(ct);
            var wanted = levelIds.ToHashSet();
            var existing = previous.Select(x => x.Level.Id).ToHashSet();
            GetRepository().RemoveRange(previous.Where(x => !wanted.Contains(x.Level.Id)));
            foreach (var id in wanted.Except(existing))
                GetRepository().Add(NewMembership(user, await RequireAsync<PlaningLevel>(id, "error.PlaningLevel.idIsNotValid", ct)));
            await R.Db.SaveChangesAsync(ct);
        }, ct);

    public Task<List<PlaningLevelUser>> CreateAsync(PlaningLevelUserDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(dto);
            var user = await R.External.GetUserAsync(RequireUserId(dto), ct);
            var result = new List<PlaningLevelUser>();
            foreach (var id in dto.LevelIds)
                result.Add(NewMembership(user, await RequireAsync<PlaningLevel>(id, "error.PlaningLevel.idIsNotValid", ct)));
            return await SaveAllAsync(result, ct);
        }, ct);

    public Task EditAsync(PlaningLevelUserDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return CreateOrDeleteAsync(RequireUserId(dto), dto.LevelIds.ToList(), ct);
    }

    private static long RequireUserId(PlaningLevelUserDto dto)
        => dto.UserId ?? throw new ArgumentException("UserId is required.", nameof(dto));

    public async Task<bool> DeleteByUserIdAsync(long userId, CancellationToken ct = default)
    {
        try
        {
            return await R.TransactionAsync(async () =>
            {
                GetRepository().RemoveRange(await GetRepository().Where(x => x.UserId == userId).ToListAsync(ct));
                await R.Db.SaveChangesAsync(ct);
                return true;
            }, ct);
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception) { throw new BusinessException("خطا در حذف planingLevelUser"); }
    }

    public async Task<bool> HasAccessInLevelIdAsync(long? levelId, long userId, CancellationToken ct = default)
    {
        if (R.User.IsAdmin) return true;
        if (levelId.HasValue && await GetRepository().AnyAsync(x => x.Level.Id == levelId.Value && x.UserId == userId, ct)) return true;
        throw new UnAuthorizedException();
    }

    public async Task HasAccessInLevelIdsAsync(List<long> levelIds, long userId, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(levelIds);
        if (R.User.IsAdmin) return;
        var allowed = await GetRepository().Where(x => x.UserId == userId && levelIds.Contains(x.Level.Id)).Select(x => x.Level.Id).Distinct().ToListAsync(ct);
        if (levelIds.Except(allowed).Any())
            throw new BusinessException("کاربر گرامی، شما دسترسی لازم به تمام کمیته‌های انتخاب شده را ندارید.");
    }

    // A unique database constraint on (LevelId, UserId) is also required for concurrent callers.
    public Task CreateIfNotExistsAsync(long levelId, long userId, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            if (await GetRepository().AnyAsync(x => x.Level.Id == levelId && x.UserId == userId, ct)) return;
            var level = await RequireAsync<PlaningLevel>(levelId, "error.PlaningLevel.idIsNotValid", ct);
            var user = await R.External.GetUserAsync(userId, ct);
            await SaveAsync(NewMembership(user, level), ct);
        }, ct);

    private PlaningLevelUser NewMembership(User user, PlaningLevel level)
    {
        var now = R.Clock.GetUtcNow().UtcDateTime;
        return new PlaningLevelUser { User = user, Level = level, CreatedDate = now, ModifiedDate = now };
    }
}
