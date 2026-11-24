using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Persistence;

public sealed class UserAccountRepository(AppDbContext db) : IUserAccountRepository
{
    public Task<UserAccount?> FindJdbcAccountAsync(long tenantId, string username, CancellationToken cancellationToken = default) =>
        db.UserAccounts
            .Include(account => account.User)
            .Include(account => account.Provider)
            .Where(account => account.AccountName == username && account.Provider != null && account.Provider.TenantId == tenantId
                && account.User != null && account.User.TenantId == tenantId)
            .Where(account => account.Provider!.Type == "JDBC" || account.Provider.Type == "DAO")
            .SingleOrDefaultAsync(cancellationToken);

    public Task<UserAccount?> FindByUsernameAsync(long tenantId, string username, CancellationToken cancellationToken = default) =>
        db.UserAccounts
            .Include(account => account.User)
            .Include(account => account.Provider)
            .Where(account => account.AccountName == username &&
                              account.Provider != null &&
                              account.Provider.TenantId == tenantId)
            .OrderBy(account => account.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<UserAccount?> FindByProviderAndUsernameAsync(long tenantId, long providerId, string username, CancellationToken cancellationToken = default) =>
        db.UserAccounts
            .Include(account => account.User)
            .Include(account => account.Provider)
            .Where(account => account.ProviderId == providerId && account.Provider != null &&
                              account.Provider.TenantId == tenantId &&
                              account.AccountName.ToUpper() == username.ToUpper())
            .SingleOrDefaultAsync(cancellationToken);

    public Task<UserAccount?> FindByUserIdAsync(long tenantId, long userId, CancellationToken cancellationToken = default) =>
        db.UserAccounts
            .Include(account => account.User)
            .Include(account => account.Provider)
            .Where(account => account.UserId == userId && account.User != null && account.User.TenantId == tenantId)
            .OrderBy(account => account.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task AddAsync(UserAccount account, CancellationToken cancellationToken = default)
    {
        db.UserAccounts.Add(account);
        await db.SaveChangesAsync(cancellationToken);
    }
}
