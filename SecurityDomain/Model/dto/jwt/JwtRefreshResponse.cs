using SemBackDotNet10.SecurityDomain.Model.SemUserDetail;

namespace SemBackDotNet10.SecurityDomain.Model.dto.jwt;

/// <summary>
/// معادل JwtRefreshResponse.java — بدنه‌ی پاسخ refresh توکن.
/// وابستگی اعلام‌شده: Token (از securityDomain.model.semUserDetail — هنوز تبدیل نشده؛
/// وقتی فایل جاوای Token.java فرستاده شود معادل آن ساخته می‌شود).
/// </summary>
public class JwtRefreshResponse
{
    public JwtRefreshResponse(Token accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public Token AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>معادل مقدار پیش‌فرض "Bearer".</summary>
    public string TokenType { get; set; } = "Bearer";
}
