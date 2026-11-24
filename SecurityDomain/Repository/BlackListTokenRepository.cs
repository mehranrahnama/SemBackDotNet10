using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Repository;

public interface IBlackListTokenRepository
{
    Task<BlackListToken?> GetBlackListTokenByTokenAsync(string token, CancellationToken ct = default);

    Task DeleteAllByAccountAsync(UserAccount account, CancellationToken ct = default);

    Task<int> DeleteByAccountAsync(UserAccount account, CancellationToken ct = default);

    Task<List<BlackListToken>> FindAllByAccountIdAsync(long id, CancellationToken ct = default);

    Task<BlackListToken> AddAsync(BlackListToken entity, CancellationToken ct = default);

    Task DeleteAsync(BlackListToken entity, CancellationToken ct = default);

    Task<List<BlackListToken>> FindAllAsync(CancellationToken ct = default);
}

public class BlackListTokenRepository : SecurityRepositoryBase<BlackListToken>, IBlackListTokenRepository
{
    public BlackListTokenRepository(AppDbContext context) : base(context) { }

    public Task<BlackListToken?> GetBlackListTokenByTokenAsync(string token, CancellationToken ct = default) =>
        Set.FirstOrDefaultAsync(b => b.Token == token, ct);

    public async Task DeleteAllByAccountAsync(UserAccount account, CancellationToken ct = default) =>
        await DeleteByAccountAsync(account, ct);

    public async Task<int> DeleteByAccountAsync(UserAccount account, CancellationToken ct = default)
    {
        var tokens = await Set.Where(b => b.Account.Id == account.Id).ToListAsync(ct);
        Set.RemoveRange(tokens);
        await Context.SaveChangesAsync(ct);
        return tokens.Count;
    }

    public Task<List<BlackListToken>> FindAllByAccountIdAsync(long id, CancellationToken ct = default) =>
        Set.Where(b => b.Account.Id == id).ToListAsync(ct);
}
