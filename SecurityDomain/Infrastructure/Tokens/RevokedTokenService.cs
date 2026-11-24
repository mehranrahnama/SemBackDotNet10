using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Tokens;

public sealed class RevokedTokenService(AppDbContext db) : IRevokedTokenService
{
    public Task<bool> IsRevokedAsync(string token, long tenantId, CancellationToken cancellationToken = default) =>
        db.RevokedTokens.AnyAsync(revoked => revoked.TokenHash == Hash(token) &&
                                           revoked.TenantId == tenantId &&
                                           revoked.ExpiresAt > DateTimeOffset.UtcNow,
            cancellationToken);

    public async Task RevokeAsync(string token, long tenantId, DateTimeOffset expiresAt, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token) || expiresAt <= DateTimeOffset.UtcNow)
            return;

        var hash = Hash(token);
        var exists = await db.RevokedTokens.AnyAsync(
            revoked => revoked.TokenHash == hash && revoked.TenantId == tenantId,
            cancellationToken);
        if (exists)
            return;

        db.RevokedTokens.Add(new RevokedToken
        {
            TokenHash = hash,
            TenantId = tenantId,
            ExpiresAt = expiresAt,
            RevokedAt = DateTimeOffset.UtcNow
        });
        await db.SaveChangesAsync(cancellationToken);
    }

    private static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
