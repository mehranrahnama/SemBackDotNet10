using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshTokenResult> CreateAsync(long tenantId, long accountId, CancellationToken cancellationToken = default);

    Task<bool> RevokeAsync(string token, CancellationToken cancellationToken = default);

    Task<RefreshTokenResult?> RotateAsync(
        string token,
        long tenantId,
        long accountId,
        CancellationToken cancellationToken = default);

    Task<bool> ValidateAsync(string token, long tenantId, long accountId, CancellationToken cancellationToken = default);
}
