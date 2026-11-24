using global::SemBackDotNet10.SecurityDomain.Service.Interf;
using SemLogger = global::Common.logging.SemLogger;
using Microsoft.Extensions.Configuration;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;

namespace SemBackDotNet10.SecurityDomain.Service.Impl;

/// <summary>
/// معادل RefreshTokenServiceimpl.java.
/// وابستگی‌های اعلام‌شده: IRefreshTokenRepository (تبدیل شده)، IUserAccountService، SemLogger (هنوز تبدیل نشده).
/// @Value("${sem.jwt.refreshExpirationMs}") → کلید "sem:jwt:refreshExpirationMs" در IConfiguration.
/// مدت اعتبار از تنظیمات سرویس خوانده می‌شود و در موجودیت ذخیره نمی‌شود.
/// </summary>
public class RefreshTokenServiceImpl : IRefreshTokenService
{
    private readonly long _refreshTokenDurationMs;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserAccountService _accountService;
    private readonly SemLogger _semLogger;

    public RefreshTokenServiceImpl(
        IConfiguration configuration,
        IRefreshTokenRepository refreshTokenRepository,
        IUserAccountService accountService,
        SemLogger semLogger)
    {
        _refreshTokenDurationMs = long.Parse(configuration["sem:jwt:refreshExpirationMs"] ?? "900000");
        _refreshTokenRepository = refreshTokenRepository;
        _accountService = accountService;
        _semLogger = semLogger;
    }

    public long RefreshTokenDurationMilliseconds => _refreshTokenDurationMs;

    /// <summary>معادل getRepository().</summary>
    public IRefreshTokenRepository GetRepository() => _refreshTokenRepository;

    public async Task<LegacyRefreshSession?> FindByTokenAsync(string token, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        var result = await _refreshTokenRepository.FindByTokenAsync(token, ct);
        _semLogger.Info("Exit method");
        return result;
    }

    public Task<LegacyRefreshSession?> FindByRefreshTokenAsync(string token, CancellationToken ct = default) =>
        _refreshTokenRepository.FindByRefreshTokenAsync(token, ct);

    public async Task<LegacyRefreshSession> CreateRefreshTokenAsync(long accountId, CancellationToken ct = default)
    {
        var refreshToken = await CreateBaseRefreshTokenAsync(accountId, ct);
        var value = Guid.NewGuid().ToString();
        refreshToken.IssuedRefreshToken = value;
        refreshToken.RefreshTokenHash = LegacyRefreshSession.Hash(value);
        await _refreshTokenRepository.AddAsync(refreshToken, ct);
        return refreshToken;
    }

    public async Task<LegacyRefreshSession> CreateRefreshTokenAsync(long accountId, string token, CancellationToken ct = default)
    {
        var refreshToken = await CreateBaseRefreshTokenAsync(accountId, ct);
        var value = Guid.NewGuid().ToString();
        refreshToken.IssuedRefreshToken = value;
        refreshToken.RefreshTokenHash = LegacyRefreshSession.Hash(value);
        refreshToken.AccessTokenHash = LegacyRefreshSession.Hash(token);
        await _refreshTokenRepository.AddAsync(refreshToken, ct);
        return refreshToken;
    }

    private async Task<LegacyRefreshSession> CreateBaseRefreshTokenAsync(long accountId, CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;
        var duration = now.ToUnixTimeMilliseconds() + _refreshTokenDurationMs;
        var expiryDate = DateTimeOffset.FromUnixTimeMilliseconds(duration);
        var account = await _accountService.FindByIdOrElseThrowAsync(accountId, ct);
        return new LegacyRefreshSession
        {
            AccountId = account.Id,
            ExpiresAt = expiryDate
        };
    }

    /// <summary>اگر منقضی شده باشد حذف و null برمی‌گرداند — معادل verifyExpiration.</summary>
    public async Task<LegacyRefreshSession?> VerifyExpirationAsync(LegacyRefreshSession token, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        if (token.ExpiresAt.CompareTo(DateTimeOffset.UtcNow) < 0)
        {
            await _refreshTokenRepository.DeleteAsync(token, ct);
            _semLogger.Info("Exit method");
            return null;
        }
        _semLogger.Info("Exit method");
        return token;
    }

    /// <summary>معادل deleteByAccountId با @Transactional — حذف با stored procedure جاوا معادل DeleteByAccountAsync است.</summary>
    public async Task<int> DeleteByAccountIdAsync(long id, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        var existingRefreshToken = await _refreshTokenRepository.FindByAccountIdAsync(id, ct);
        if (existingRefreshToken is not null)
        {
            var result = await _refreshTokenRepository.DeleteByAccountAsync(
                await _accountService.FindByIdOrElseThrowAsync(id, ct), ct);
            _semLogger.Info("Exit method");
            return result;
        }
        _semLogger.Info("Exit method");
        return 0;
    }

    public async Task DeleteAllByAccountIdAsync(long id, CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        var tokenList = await _refreshTokenRepository.FindAllByAccountIdAsync(id, ct);
        if (tokenList is { Count: > 0 })
        {
            foreach (var token in tokenList)
            {
                await _refreshTokenRepository.DeleteAsync(token, ct);
            }
        }
        _semLogger.Info("Exit method");
    }

    public async Task RemoveExpiredTokensAsync(CancellationToken ct = default)
    {
        _semLogger.Info("Enter the method");
        var tokenList = await _refreshTokenRepository.FindAllAsync(ct);
        foreach (var token in tokenList)
        {
            await VerifyExpirationAsync(token, ct);
        }
        _semLogger.Info("Exit method");
    }

    /// <summary>معادل findAllByAccountId که در جاوا برای لیست خالی null برمی‌گرداند.</summary>
    public async Task<List<LegacyRefreshSession>?> FindAllByAccountIdAsync(long accountId, CancellationToken ct = default)
    {
        var result = await _refreshTokenRepository.FindAllByAccountIdAsync(accountId, ct);
        return result.Count > 0 ? result : null;
    }
}
