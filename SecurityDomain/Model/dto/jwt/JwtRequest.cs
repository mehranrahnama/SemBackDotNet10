using System.ComponentModel.DataAnnotations;

namespace SemBackDotNet10.SecurityDomain.Model.dto.jwt;

/// <summary>
/// معادل JwtRequest.java — بدنه‌ی درخواست لاگین.
/// jakarta.validation @NotBlank با پیام → [Required(ErrorMessage = "...")].
/// Serializable جاوا در دات‌نت نیاز به معادل ندارد.
/// </summary>
public class JwtRequest
{
    public JwtRequest()
    {
    }

    public JwtRequest(string username, string password)
    {
        Username = username;
        Password = password;
    }

    /// <summary>معادل @NotBlank(message = "error.user.usernameIsNotValid").</summary>
    [Required(ErrorMessage = "error.user.usernameIsNotValid")]
    public string Username { get; set; } = string.Empty;

    /// <summary>معادل @NotBlank(message = "error.user.passwordIsNotValid").</summary>
    [Required(ErrorMessage = "error.user.passwordIsNotValid")]
    public string Password { get; set; } = string.Empty;

    public long? CurrentTenant { get; set; }
}
