namespace SemBackDotNet10.SecurityDomain.Model.dto.jwt;

/// <summary>
/// معادل JwtResponse.java — بدنه‌ی پاسخ لاگین.
/// فیلد final maxAge جاوا → property با init-only setter.
/// </summary>
public class JwtResponse
{
    public JwtResponse(long maxAge, long? factoryId, string accessToken, string refreshToken)
    {
        MaxAge = maxAge;
        FactoryId = factoryId;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    /// <summary>معادل private final Long maxAge — فقط در سازنده مقدار می‌گیرد.</summary>
    public long MaxAge { get; init; }

    public long? FactoryId { get; set; }

    public string AccessToken { get; set; } = string.Empty;

    /// <summary>معادل مقدار پیش‌فرض "Bearer".</summary>
    public string Type { get; set; } = "Bearer";

    public string RefreshToken { get; set; } = string.Empty;
}
