using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SemBackDotNet10.SecurityDomain.Authentication;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Authentication;

public sealed class LocalTokenService(AppDbContext db, IConfiguration configuration, IRoleRepository roles)
{
    public const string Issuer = "sem-local";

    public async Task<IssuedSession> CreateAsync(AuthenticationResult authentication, CancellationToken cancellationToken)
    {
        var issued = Issue(authentication);
        await db.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            db.ChangeTracker.Clear();
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var existing = await db.Set<LegacyRefreshSession>().Where(item => item.AccountId == authentication.AccountId).ToListAsync(cancellationToken);
            db.RemoveRange(existing);
            db.Add(issued.Session);
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
        return issued;
    }

    public async Task<IssuedSession?> RotateAsync(string refreshToken, long tenantId, CancellationToken cancellationToken)
    {
        return await db.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            db.ChangeTracker.Clear();
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var hash = LegacyRefreshSession.Hash(refreshToken);
            var session = await db.Set<LegacyRefreshSession>().SingleOrDefaultAsync(item => item.RefreshTokenHash == hash, cancellationToken);
            if (session is null || session.ExpiresAt <= DateTimeOffset.UtcNow)
                return null;

            var account = await db.UserAccounts.Include(item => item.User).Include(item => item.Provider)
                .SingleOrDefaultAsync(item => item.Id == session.AccountId, cancellationToken);
            if (account?.User is null || account.Provider is null || account.User.TenantId != tenantId ||
                account.Provider.TenantId != tenantId || account.User.Retired == true || account.User.LoginDisabled == true)
                return null;

            var currentRoles = await roles.GetForUserAsync(tenantId, account.User.UserId, cancellationToken);
            var authentication = AuthenticationResult.Success(tenantId, account.AccountName, null, account.Provider.Type,
                account.User.UserId, account.Id, currentRoles.Select(role => role.Name).ToArray());
            var issued = Issue(authentication);
            session.AccessTokenHash = issued.Session.AccessTokenHash;
            session.RefreshTokenHash = issued.Session.RefreshTokenHash;
            session.ExpiresAt = issued.Session.ExpiresAt;
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return issued;
        });
    }

    public async Task RevokeAsync(string accessToken, long accountId, CancellationToken cancellationToken)
    {
        var hash = LegacyRefreshSession.Hash(accessToken);
        var session = await db.Set<LegacyRefreshSession>().SingleOrDefaultAsync(item => item.AccountId == accountId && item.AccessTokenHash == hash, cancellationToken);
        if (session is null) return;
        db.Remove(session);
        await db.SaveChangesAsync(cancellationToken);
    }

    private IssuedSession Issue(AuthenticationResult authentication)
    {
        var secret = configuration["sem:jwt:secret"] ?? throw new InvalidOperationException("JWT secret is not configured.");
        var audience = configuration["Authentication:Audience"] ?? throw new InvalidOperationException("JWT audience is not configured.");
        var accessMilliseconds = configuration.GetValue<long?>("sem:jwt:expiration") ?? 900000;
        var refreshMilliseconds = configuration.GetValue<long?>("sem:jwt:refreshExpirationMs") ?? 3600000;
        if (accessMilliseconds <= 0 || refreshMilliseconds <= 0 || authentication.UserId is null || authentication.AccountId is null)
            throw new InvalidOperationException("Invalid token lifetime or authenticated identity.");
        var now = DateTime.UtcNow;
        var expires = now.AddMilliseconds(accessMilliseconds);
        var id = Guid.NewGuid().ToString();
        var subject = authentication.UserId.Value.ToString();
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, subject), new(JwtRegisteredClaimNames.Jti, id),
            new("username", authentication.Username), new("tenant_id", authentication.TenantId.ToString()),
            new("account_id", authentication.AccountId.Value.ToString())
        };
        claims.AddRange((authentication.Roles ?? []).Select(role => new Claim("role", role)));
        var jwt = new JwtSecurityToken(Issuer, audience, claims, now, expires,
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)), SecurityAlgorithms.HmacSha256));
        var access = new JwtSecurityTokenHandler().WriteToken(jwt);
        var refresh = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var session = new LegacyRefreshSession
        {
            AccountId = authentication.AccountId.Value,
            AccessTokenHash = LegacyRefreshSession.Hash(access), RefreshTokenHash = LegacyRefreshSession.Hash(refresh),
            ExpiresAt = DateTimeOffset.UtcNow.AddMilliseconds(refreshMilliseconds)
        };
        return new IssuedSession(access, refresh, accessMilliseconds, refreshMilliseconds, expires, subject, id, session);
    }
}

public sealed record IssuedSession(string AccessToken, string RefreshToken, long AccessMilliseconds,
    long RefreshMilliseconds, DateTime AccessExpiresAt, string Subject, string JwtId, LegacyRefreshSession Session);
