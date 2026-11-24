using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;

namespace SemBackDotNet10.SecurityDomain.Service.Interf;

/// <summary>
/// معادل BlackListTokenService.java — اینترفیس سرویس توکن‌های سیاه‌لیست.
/// BaseService جاوا (CRUD عمومی) با متد GetRepository شبیه‌سازی شده است.
/// </summary>
public interface IBlackListTokenService
{
    /// <summary>معادل متد BaseService: getRepository().</summary>
    IBlackListTokenRepository GetRepository();

    /// <summary>معادل findByToken(String).</summary>
    Task<BlackListToken?> FindByTokenAsync(string token, CancellationToken ct = default);

    /// <summary>معادل createBlackListToken(Long, String).</summary>
    Task<BlackListToken> CreateBlackListTokenAsync(long accountId, string token, CancellationToken ct = default);

    /// <summary>معادل createBlackListToken(Long, List&lt;String&gt;).</summary>
    Task<List<BlackListToken>> CreateBlackListTokensAsync(long accountId, List<string> tokens, CancellationToken ct = default);

    /// <summary>معادل deleteByToken(String) با @Transactional.</summary>
    Task DeleteByTokenAsync(string token, CancellationToken ct = default);

    /// <summary>معادل deleteAllByAccountId(Long) با @Transactional.</summary>
    Task DeleteAllByAccountIdAsync(long id, CancellationToken ct = default);

    /// <summary>معادل removeExpiredTokens() با @Scheduled.</summary>
    Task RemoveExpiredTokensAsync(CancellationToken ct = default);

    /// <summary>معادل findAllByAccountId(Long).</summary>
    Task<List<BlackListToken>> FindAllByAccountIdAsync(long id, CancellationToken ct = default);

    /// <summary>معادل verifyExpiration(BlackListToken).</summary>
    Task<BlackListToken?> VerifyExpirationAsync(BlackListToken token, CancellationToken ct = default);

    /// <summary>معادل saveAllRefreshTokensToBlackList(List&lt;LegacyRefreshSession&gt;).</summary>
    Task SaveAllRefreshTokensToBlackListAsync(List<LegacyRefreshSession>? usersToken, CancellationToken ct = default);
}
