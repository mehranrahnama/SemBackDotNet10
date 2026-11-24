using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Repository;

public interface IRefreshTokenRepository
{
    Task<LegacyRefreshSession?> FindByTokenAsync(string token, CancellationToken ct = default);

    Task<LegacyRefreshSession?> FindByRefreshTokenAsync(string token, CancellationToken ct = default);

    Task<LegacyRefreshSession?> FindByAccountIdAsync(long accountId, CancellationToken ct = default);

    Task<int> DeleteByAccountAsync(UserAccount account, CancellationToken ct = default);

    Task<List<LegacyRefreshSession>> FindAllByAccountIdAsync(long accountId, CancellationToken ct = default);

    Task<LegacyRefreshSession> AddAsync(LegacyRefreshSession entity, CancellationToken ct = default);

    Task DeleteAsync(LegacyRefreshSession entity, CancellationToken ct = default);

    Task<List<LegacyRefreshSession>> FindAllAsync(CancellationToken ct = default);
}

public class RefreshTokenRepository : SecurityRepositoryBase<LegacyRefreshSession>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context) { }

    public Task<LegacyRefreshSession?> FindByTokenAsync(string token, CancellationToken ct = default) =>
        Set.FirstOrDefaultAsync(r => r.AccessTokenHash == LegacyRefreshSession.Hash(token), ct);

    public Task<LegacyRefreshSession?> FindByRefreshTokenAsync(string token, CancellationToken ct = default) =>
        Set.FirstOrDefaultAsync(r => r.RefreshTokenHash == LegacyRefreshSession.Hash(token), ct);

    public Task<LegacyRefreshSession?> FindByAccountIdAsync(long accountId, CancellationToken ct = default) =>
        Set.FirstOrDefaultAsync(r => r.AccountId == accountId, ct);

    public async Task<int> DeleteByAccountAsync(UserAccount account, CancellationToken ct = default)
    {
        var items = await Set.Where(r => r.AccountId == account.Id).ToListAsync(ct);
        Set.RemoveRange(items);
        await Context.SaveChangesAsync(ct);
        return items.Count;
    }

    public Task<List<LegacyRefreshSession>> FindAllByAccountIdAsync(long accountId, CancellationToken ct = default) =>
        Set.Where(r => r.AccountId == accountId && r.RefreshTokenHash != null && r.RefreshTokenHash.StartsWith(LegacyRefreshSession.HashPrefix)).ToListAsync(ct);
}
