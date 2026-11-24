using System.ComponentModel.DataAnnotations;

namespace SemBackDotNet10.SecurityDomain.Model.dto.jwt;

/// <summary>
/// معادل JwtRefreshRequest.java — بدنه‌ی درخواست refresh توکن.
/// jakarta.validation @NotBlank → [Required].
/// </summary>
public class JwtRefreshRequest
{
    /// <summary>معادل @NotBlank.</summary>
    [Required(ErrorMessage = "error.user.session.expire")]
    public string RefreshToken { get; set; } = string.Empty;

    public long? CurOrgan { get; set; }

    public string? OriginalUser { get; set; }
}
