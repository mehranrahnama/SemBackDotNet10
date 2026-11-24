using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;

namespace SemBackDotNet10.SecurityDomain.Service.Interf;

/// <summary>معادل RefreshTokenService.java — اینترفیس سرویس refresh token.</summary>
public interface IRefreshTokenService
{
    long RefreshTokenDurationMilliseconds { get; }
    /// <summary>معادل متد BaseService: getRepository().</summary>
    IRefreshTokenRepository GetRepository();

    /// <summary>معادل findByToken(String).</summary>
    Task<LegacyRefreshSession?> FindByTokenAsync(string token, CancellationToken ct = default);

    Task<LegacyRefreshSession?> FindByRefreshTokenAsync(string token, CancellationToken ct = default);

    /// <summary>معادل createRefreshToken(Long).</summary>
    Task<LegacyRefreshSession> CreateRefreshTokenAsync(long accountId, CancellationToken ct = default);

    /// <summary>معادل createRefreshToken(Long, String).</summary
    /// </summary>
    Task<LegacyRefreshSession> CreateRefreshTokenAsync(long accountId, string token, CancellationToken ct = default);

    /// <summary>معادل verifyExpiration(LegacyRefreshSession).</summary>
    Task<LegacyRefreshSession?> VerifyExpirationAsync(LegacyRefreshSession token, CancellationToken ct = default);

    /// <summary>معادل deleteByAccountId(Long) با @Transactional.</summary>
    Task<int> DeleteByAccountIdAsync(long id, CancellationToken ct = default);

    /// <summary>معادل deleteAllByAccountId(Long) با @Transactional.</summary>
    Task DeleteAllByAccountIdAsync(long id, CancellationToken ct = default);

    /// <summary>معادل removeExpiredTokens() با @Scheduled.</summary>
    Task RemoveExpiredTokensAsync(CancellationToken ct = default);

    /// <summary>معادل findAllByAccountId(Long) — در جاوا لیست خالی null برمی‌گرداند.</summary>
    Task<List<LegacyRefreshSession>?> FindAllByAccountIdAsync(long accountId, CancellationToken ct = default);
}
