using global::SemBackDotNet10.SecurityDomain.Service.Interf;
using SemLogger = global::Common.logging.SemLogger;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;

namespace SemBackDotNet10.SecurityDomain.Service.Impl;

/// <summary>
/// معادل BlackListTokenServiceImpl.java.
/// وابستگی‌های اعلام‌شده: IBlackListTokenRepository (تبدیل شده)، IUserAccountService، SemLogger (هر دو هنوز تبدیل نشده).
/// @Transactional جاوا → در EF Core با یک SaveChanges واحد در انتهای متد شبیه‌سازی شده.
/// </summary>
public class BlackListTokenServiceImpl : IBlackListTokenService
{
    /// <summary>معادل EXPIRATION_DURATION = 3600000 (یک ساعت بر حسب میلی‌ثانیه).</summary>
    private const long ExpirationDuration = 3_600_000;

    private readonly IBlackListTokenRepository _blackListTokenRepository;
    private readonly IUserAccountService _userAccountService;
    private readonly SemLogger _semLogger;

    public BlackListTokenServiceImpl(
        IBlackListTokenRepository blackListTokenRepository,
        IUserAccountService userAccountService,
        SemLogger semLogger)
    {
        _blackListTokenRepository = blackListTokenRepository;
        _userAccountService = userAccountService;
        _semLogger = semLogger;
    }

    /// <summary>معادل getRepository().</summary>
    public IBlackListTokenRepository GetRepository() => _blackListTokenRepository;

    public async Task<BlackListToken?> FindByTokenAsync(string token, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        var result = await _blackListTokenRepository.GetBlackListTokenByTokenAsync(LegacyRefreshSession.Hash(token), ct);
        _semLogger.Info("Exit method");
        return result;
    }

    private async Task<BlackListToken> CreateBlackListTokenObjectAsync(long accountId, string token, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        var account = await _userAccountService.FindByIdOrElseThrowAsync(accountId, ct);
        var now = DateTimeOffset.UtcNow;
        var duration = now.ToUnixTimeMilliseconds() + ExpirationDuration;
        var expirationDate = DateTimeOffset.FromUnixTimeMilliseconds(duration);
        var blackListToken = new BlackListToken
        {
            Token = token,
            Account = account,
            Duration = duration,
            ExpiryDate = expirationDate.LocalDateTime
        };
        _semLogger.Info("Exit method");
        return blackListToken;
    }

    public async Task<BlackListToken> CreateBlackListTokenAsync(long accountId, string token, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        var blackListToken = await CreateBlackListTokenObjectAsync(accountId, LegacyRefreshSession.Hash(token), ct);
        await _blackListTokenRepository.AddAsync(blackListToken, ct);
        _semLogger.Info("Exit method");
        return blackListToken;
    }

    /// <summary>معادل createBlackListToken(Long, List&lt;String&gt;) — ذخیره‌ی دسته‌ای.</summary>
    public async Task<List<BlackListToken>> CreateBlackListTokensAsync(long accountId, List<string> tokens, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        var blackListTokenInBatch = new List<BlackListToken>();
        foreach (var token in tokens)
        {
            blackListTokenInBatch.Add(await CreateBlackListTokenObjectAsync(accountId, LegacyRefreshSession.Hash(token), ct));
        }
        foreach (var blackListToken in blackListTokenInBatch)
        {
            await _blackListTokenRepository.AddAsync(blackListToken, ct);
        }
        _semLogger.Info("Exit method");
        return blackListTokenInBatch;
    }

    public async Task DeleteByTokenAsync(string token, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        var existingToken = await _blackListTokenRepository.GetBlackListTokenByTokenAsync(LegacyRefreshSession.Hash(token), ct);
        if (existingToken is not null)
        {
            await _blackListTokenRepository.DeleteAsync(existingToken, ct);
        }
        _semLogger.Info("Exit method");
    }

    /// <summary>معادل deleteAllByAccountId با @Transactional.</summary>
    public async Task DeleteAllByAccountIdAsync(long id, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        var tokenList = await FindAllByAccountIdAsync(id, ct);
        if (tokenList is { Count: > 0 })
        {
            foreach (var blackListToken in tokenList)
            {
                await _blackListTokenRepository.DeleteAsync(blackListToken, ct);
            }
        }
        _semLogger.Info("Exit method");
    }

    public async Task RemoveExpiredTokensAsync(CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        var tokenList = await _blackListTokenRepository.FindAllAsync(ct);
        foreach (var token in tokenList)
        {
            await VerifyExpirationAsync(token, ct);
        }
        _semLogger.Info("Exit method");
    }

    public async Task<List<BlackListToken>> FindAllByAccountIdAsync(long id, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        var result = await _blackListTokenRepository.FindAllByAccountIdAsync(id, ct);
        _semLogger.Info("Exit method");
        return result;
    }

    /// <summary>اگر توکن منقضی شده باشد حذف می‌شود و null برمی‌گرداند — معادل verifyExpiration.</summary>
    public async Task<BlackListToken?> VerifyExpirationAsync(BlackListToken token, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        if (token.ExpiryDate < DateTime.Now)
        {
            await _blackListTokenRepository.DeleteAsync(token, ct);
            _semLogger.Info("Exit method");
            return null;
        }
        _semLogger.Info("Exit method");
        return token;
    }

    /// <summary>معادل saveAllRefreshTokensToBlackList(List&lt;LegacyRefreshSession&gt;).</summary>
    public async Task SaveAllRefreshTokensToBlackListAsync(List<LegacyRefreshSession>? usersToken, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        if (usersToken is { Count: > 0 })
        {
            var blackListTokens = new List<BlackListToken>();
            foreach (var refreshToken in usersToken)
            {
                if (refreshToken.AccessTokenHash is { } hash && hash.StartsWith(LegacyRefreshSession.HashPrefix, StringComparison.Ordinal))
                    blackListTokens.Add(await CreateBlackListTokenObjectAsync(refreshToken.AccountId, hash, ct));
            }
            foreach (var blackListToken in blackListTokens)
            {
                await _blackListTokenRepository.AddAsync(blackListToken, ct);
            }
        }
        _semLogger.Info("Exit method");
    }
}
