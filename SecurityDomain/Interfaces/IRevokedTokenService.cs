namespace SemBackDotNet10.SecurityDomain.Interfaces;

public interface IRevokedTokenService
{
    Task<bool> IsRevokedAsync(string token, long tenantId, CancellationToken cancellationToken = default);

    Task RevokeAsync(string token, long tenantId, DateTimeOffset expiresAt, CancellationToken cancellationToken = default);
}
