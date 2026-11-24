using System.Security.Cryptography;
using System.Text;
using System.Data;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Persistence;
using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Tokens;

public sealed class RefreshTokenService(AppDbContext db, IConfiguration configuration) : IRefreshTokenService
{
    public async Task<RefreshTokenResult> CreateAsync(long tenantId, long accountId, CancellationToken cancellationToken = default)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiresAt = DateTimeOffset.UtcNow.Add(GetRefreshTokenLifetime());
        db.RefreshTokens.Add(new RefreshToken
        {
            TenantId = tenantId,
            AccountId = accountId,
            FamilyId = Guid.NewGuid(),
            TokenHash = Hash(token),
            ExpiresAt = expiresAt
        });
        await db.SaveChangesAsync(cancellationToken);
        return new RefreshTokenResult(token, expiresAt);
    }

    public async Task<RefreshTokenResult?> RotateAsync(
        string token,
        long tenantId,
        long accountId,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);
        var current = await db.RefreshTokens.SingleOrDefaultAsync(
            item => item.TokenHash == Hash(token) &&
                    item.TenantId == tenantId &&
                    item.AccountId == accountId,
            cancellationToken);

        if (current is null)
            return null;

        if (current.RevokedAt.HasValue || current.ConsumedAt.HasValue || current.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            await RevokeFamilyAsync(current.FamilyId, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return null;
        }

        var replacement = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var replacementHash = Hash(replacement);
        var expiresAt = DateTimeOffset.UtcNow.Add(GetRefreshTokenLifetime());
        current.ConsumedAt = DateTimeOffset.UtcNow;
        current.RevokedAt = DateTimeOffset.UtcNow;
        current.ReplacedByHash = replacementHash;
        db.RefreshTokens.Add(new RefreshToken
        {
            TenantId = tenantId,
            AccountId = accountId,
            FamilyId = current.FamilyId,
            TokenHash = replacementHash,
            ExpiresAt = expiresAt
        });
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new RefreshTokenResult(replacement, expiresAt);
    }

    public async Task<bool> RevokeAsync(string token, CancellationToken cancellationToken = default)
    {
        var entity = await db.RefreshTokens.SingleOrDefaultAsync(
            item => item.TokenHash == Hash(token) && item.RevokedAt == null,
            cancellationToken);
        if (entity is null)
            return false;

        entity.RevokedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> ValidateAsync(string token, long tenantId, long accountId, CancellationToken cancellationToken = default) =>
        db.RefreshTokens.AnyAsync(item => item.TokenHash == Hash(token) &&
                                          item.TenantId == tenantId &&
                                          item.AccountId == accountId &&
                                          item.RevokedAt == null &&
                                          item.ConsumedAt == null &&
                                          item.ExpiresAt > DateTimeOffset.UtcNow,
            cancellationToken);

    private async Task RevokeFamilyAsync(Guid familyId, CancellationToken cancellationToken)
    {
        var family = await db.RefreshTokens
            .Where(item => item.FamilyId == familyId && item.RevokedAt == null)
            .ToListAsync(cancellationToken);
        foreach (var token in family)
            token.RevokedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
    }

    private static string Hash(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    private TimeSpan GetRefreshTokenLifetime()
    {
        var milliseconds = configuration.GetValue<long?>("Jwt:RefreshTokenLifetimeMilliseconds");
        if (milliseconds is <= 0) throw new InvalidOperationException("Jwt:RefreshTokenLifetimeMilliseconds must be positive.");
        return milliseconds.HasValue ? TimeSpan.FromMilliseconds(milliseconds.Value)
            : TimeSpan.FromDays(configuration.GetValue("Jwt:RefreshTokenDays", 30));
    }
}
