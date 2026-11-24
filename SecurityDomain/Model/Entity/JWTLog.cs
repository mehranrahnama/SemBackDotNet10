namespace SemBackDotNet10.SecurityDomain.Model.Entity;

/// <summary>
/// معادل JWTLog.java — کلاس @Data لامبوك به رکورد با init-only setters تبدیل شد.
/// @RedisHash کامنت شده بود و Redis استفاده نمی‌شود؛ Serializable هم در دات‌نت معنای
/// مستقیمی ندارد و حذف شد. @Id جاوای Spring Data → کلید رکورد.
/// </summary>
public record JWTLog
{
    [System.ComponentModel.DataAnnotations.Key]
    public string JwtToken { get; init; } = null!;

    public DateTime? TokenTime { get; init; }

    public string? Account { get; init; }
}
