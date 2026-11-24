using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SemBackDotNet10.SecurityDomain.tokenprovider;

// Configuration equivalent of the Java @Value properties.
public sealed class JwtTokenProviderOptions
{
    public string Issuer { get; set; } = "";
    public long ExpirationMilliseconds { get; set; }
    public string CurrentOrgCookieName { get; set; } = "curOrgan";
    public string DefaultTenantName { get; set; } = "";
    public string AccessTokenCookieName { get; set; } = "accessToken";
    public string Secret { get; set; } = "";
}

// Adapt these contracts to the interfaces/models already present in your project.
public interface ITenantService
{
    Task<Tenant?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);
}

public interface ISemUserDetailsService
{
    Task<SemUserDetail?> LoadUserByUsernameAsync(
        string username, CancellationToken cancellationToken = default);
}

public sealed record Tenant(long Id, string Code);

public class SemUserDetail
{
    public string Username { get; init; } = "";
    public long UserId { get; init; }
    public IReadOnlyCollection<string> Authorities { get; init; } = [];
}

public sealed record ImpersonateUser(string Username, long UserId, string Password = "");

public sealed record Token(string TokenValue, long ExpiresInMilliseconds, DateTime ExpiresAt);

public sealed class UnAuthorizedException : Exception
{
    public UnAuthorizedException() : base("Unauthorized") { }
    public UnAuthorizedException(string message) : base(message) { }
}

public sealed class JwtTokenProvider
{
    private const string AuthoritiesClaim = "authorities";
    private const string UsernameClaim = "username";
    internal const string PreviousAdministratorRole = "ROLE_PREVIOUS_ADMINISTRATOR";
    private const string BearerPrefix = "Bearer ";

    private readonly JwtTokenProviderOptions _options;
    private readonly ITenantService _tenantService;
    private readonly ISemUserDetailsService _userDetailsService;
    private readonly ILogger<JwtTokenProvider> _logger;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    private SymmetricSecurityKey SigningKey =>
        new(Encoding.UTF8.GetBytes(_options.Secret));

    public JwtTokenProvider(
        IOptions<JwtTokenProviderOptions> options,
        ITenantService tenantService,
        ISemUserDetailsService userDetailsService,
        ILogger<JwtTokenProvider> logger)
    {
        _options = options.Value;
        _tenantService = tenantService;
        _userDetailsService = userDetailsService;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(_options.Secret))
            throw new InvalidOperationException("JWT secret is not configured.");
    }

    public Token IssueToken(IDictionary<string, object> claims)
    {
        ArgumentNullException.ThrowIfNull(claims);

        var now = DateTime.UtcNow;
        var expires = now.AddMilliseconds(_options.ExpirationMilliseconds);
        var jwtClaims = claims.Select(kv =>
            new Claim(kv.Key, SerializeClaimValue(kv.Value))).ToList();

        var jwt = CreateJwt(jwtClaims, now, expires);
        return new Token(jwt, _options.ExpirationMilliseconds, expires.ToLocalTime());
    }

    public Token IssueToken(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        var identity = principal.Identity as ClaimsIdentity;
        var username = principal.Identity?.Name
            ?? principal.FindFirst(UsernameClaim)?.Value
            ?? throw new ArgumentException("Principal has no username.");

        var claims = new List<Claim> { new(UsernameClaim, username) };

        // Store regular roles/authorities.
        claims.AddRange(principal.FindAll(ClaimTypes.Role)
            .Select(c => new Claim(AuthoritiesClaim, c.Value)));

        // Preserve impersonation information as a compact JSON claim.
        foreach (var impersonation in principal.FindAll("impersonation"))
            claims.Add(new Claim("impersonation", impersonation.Value));

        var now = DateTime.UtcNow;
        var expires = now.AddMilliseconds(_options.ExpirationMilliseconds);
        return new Token(CreateJwt(claims, now, expires),
            _options.ExpirationMilliseconds, expires.ToLocalTime());
    }

    private string CreateJwt(IEnumerable<Claim> claims, DateTime now, DateTime expires)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = now,
            NotBefore = now,
            Expires = expires,
            SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha512),
        };

        var token = _tokenHandler.CreateToken(descriptor);
        return _tokenHandler.WriteToken(token);
    }

    public string ExtractUsername(string token) =>
        ParseTokenAndGetClaims(token).TryGetValue(UsernameClaim, out var username)
            ? username?.ToString() ?? throw new UnAuthorizedException("Username claim is empty.")
            : throw new UnAuthorizedException("Username claim is missing.");

    public (string Subject, string Id) ReadSubjectAndId(string jwtToken)
    {
        var jwt = ReadJwtToken(jwtToken);
        return (jwt.Subject, jwt.Id);
    }

    public IDictionary<string, object> ParseTokenAndGetClaims(string jwtToken)
    {
        var jwt = ReadJwtToken(jwtToken);
        return jwt.Claims
            .GroupBy(c => c.Type)
            .ToDictionary(g => g.Key, g => (object)g.Select(c => c.Value).ToArray());
    }

    public bool ValidateAndParseToken(string jwtToken)
    {
        try
        {
            ValidateToken(jwtToken, validateIssuer: false);
            return true;
        }
        catch (SecurityTokenExpiredException)
        {
            _logger.LogError("Expired JWT token");
            throw;
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogError(ex, "Invalid JWT token");
            return false;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "JWT token is empty or invalid");
            return false;
        }
    }

    public bool CheckIsSsoToken(string token)
    {
        try
        {
            var jwt = ReadJwtToken(token);
            var tokenIssuer = jwt.Issuer;

            if (!string.Equals(tokenIssuer, _options.Issuer, StringComparison.Ordinal))
                return false;

            ValidateToken(token, validateIssuer: true);
            return true;
        }
        catch (Exception ex) when (ex is SecurityTokenException or ArgumentException)
        {
            throw new UnAuthorizedException("SSO token is invalid.");
        }
    }

    private ClaimsPrincipal ValidateToken(string token, bool validateIssuer)
    {
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = validateIssuer,
            ValidIssuer = _options.Issuer,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SigningKey,
            ClockSkew = TimeSpan.Zero,
        };

        return _tokenHandler.ValidateToken(token, parameters, out _);
    }

    // Decodes a JWT segment without validating its signature (same purpose as Java parseToken).
    public string? ParseToken(string part, string jwtToken)
    {
        try
        {
            var chunks = jwtToken.Split('.');
            if (chunks.Length < 2) return null;

            var segment = part switch
            {
                "header" => chunks[0],
                "payload" => chunks[1],
                _ => throw new ArgumentOutOfRangeException(nameof(part), part, "Expected header or payload.")
            };

            var bytes = Base64UrlEncoder.DecodeBytes(segment);
            return Encoding.UTF8.GetString(bytes);
        }
        catch (FormatException)
        {
            return null;
        }
    }

    public string? GetJwtFromRequest(HttpRequest request)
    {
        var authorization = request.Headers.Authorization.ToString();
        if (authorization.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
            return authorization[BearerPrefix.Length..].Trim();

        return null;
    }

    public async Task<string?> GetJwtTokenAsync(
        HttpRequest request, CancellationToken cancellationToken = default)
    {
        var token = GetJwtFromRequest(request);
        if (string.IsNullOrWhiteSpace(token) || !CheckIsSsoToken(token))
            return token;

        var payloadText = ParseToken("payload", token)
            ?? throw new UnAuthorizedException("SSO token payload is invalid.");
        using var payload = JsonDocument.Parse(payloadText);
        var root = payload.RootElement;

        var username = root.GetProperty(UsernameClaim).GetString()?.ToUpperInvariant()
            ?? throw new UnAuthorizedException("SSO username is missing.");
        var jti = root.GetProperty("jti").GetString();
        var subject = root.GetProperty("sub").GetString();
        var exp = root.GetProperty("exp").GetInt64();
        var iat = root.GetProperty("iat").GetInt64();

        SemUserDetail? userDetails = null;
        try
        {
            userDetails = await _userDetailsService.LoadUserByUsernameAsync(
                username + "_IRISA_LDAP", cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogError("Username not found: {Username}", username);
        }

        var claims = new List<Claim>
        {
            new(UsernameClaim, username + "_IRISA_LDAP")
        };
        if (userDetails is not null)
            claims.AddRange(userDetails.Authorities.Select(a => new Claim(ClaimTypes.Role, a)));
        else
            claims.Add(new Claim(ClaimTypes.Role, "ANONYMOUS"));

        var generated = CreateJwtWithIdentifiers(
            claims,
            subject ?? Guid.NewGuid().ToString(),
            jti ?? Guid.NewGuid().ToString(),
            DateTimeOffset.FromUnixTimeSeconds(iat).UtcDateTime,
            DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime);

        return generated;
    }

    private string CreateJwtWithIdentifiers(
        IEnumerable<Claim> claims, string subject, string jwtId, DateTime issuedAt, DateTime expires)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = expires,
            Claims = new Dictionary<string, object>
            {
                ["sub"] = subject,
                ["jti"] = jwtId
            },
            SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha512)
        };
        return _tokenHandler.WriteToken(_tokenHandler.CreateToken(descriptor));
    }

    public async Task<string?> GetCurrentOrganizationAsync(
        HttpRequest request, CancellationToken cancellationToken = default)
    {
        var currentOrganization = request.Headers[_options.CurrentOrgCookieName].FirstOrDefault();

        if (currentOrganization is null)
        {
            var tenant = await _tenantService.FindByCodeAsync(
                _options.DefaultTenantName, cancellationToken);
            if (tenant is null)
                throw new InvalidOperationException(
                    $"Default tenant '{_options.DefaultTenantName}' was not found.");

            currentOrganization = tenant.Id.ToString();
        }

        return string.IsNullOrWhiteSpace(currentOrganization) ? null : currentOrganization;
    }

    public ClaimsPrincipal GetAuthenticationPrincipal(string token, SemUserDetail userDetails)
    {
        ArgumentNullException.ThrowIfNull(userDetails);
        var jwt = ReadJwtToken(token);
        var identityClaims = new List<Claim>
        {
            new(UsernameClaim, userDetails.Username),
            new(ClaimTypes.Name, userDetails.Username)
        };

        // Restore regular authorities from token and add current user's authorities.
        identityClaims.AddRange(jwt.Claims
            .Where(c => c.Type == AuthoritiesClaim || c.Type == ClaimTypes.Role)
            .Select(c => new Claim(ClaimTypes.Role, c.Value)));
        identityClaims.AddRange(userDetails.Authorities.Select(a => new Claim(ClaimTypes.Role, a)));

        // Restore serialized impersonation authorities, if present.
        foreach (var claim in jwt.Claims.Where(c => c.Type == "impersonation"))
            identityClaims.Add(new Claim("impersonation", claim.Value));

        return new ClaimsPrincipal(new ClaimsIdentity(identityClaims, "Bearer"));
    }

    public string SetHashedCookie(string a, string b)
    {
        var combination = $"{a}:{b}";
        var hash = SHA512.HashData(Encoding.UTF8.GetBytes(combination));
        return Convert.ToBase64String(hash);
    }

    private JwtSecurityToken ReadJwtToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("JWT token is empty.", nameof(token));

        return _tokenHandler.ReadJwtToken(token);
    }

    private static string SerializeClaimValue(object? value) =>
        value switch
        {
            null => "",
            string text => text,
            _ => JsonSerializer.Serialize(value)
        };
}
