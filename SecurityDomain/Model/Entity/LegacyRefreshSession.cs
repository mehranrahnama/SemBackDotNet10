using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace SemBackDotNet10.SecurityDomain.Model.Entity;

[Table("tbl_refreshtokens", Schema = "dbo")]
public sealed class LegacyRefreshSession
{
    [Key, Column("id", TypeName = "int")]
    public long Id { get; set; }

    [Column("account_id", TypeName = "int")]
    public long AccountId { get; set; }

    [JsonIgnore, Column("token")]
    public string? AccessTokenHash { get; set; }

    [JsonIgnore, Column("refreshToken")]
    public string? RefreshTokenHash { get; set; }

    [Column("expiryDate", TypeName = "datetime")]
    public DateTimeOffset ExpiresAt { get; set; }

    [NotMapped, JsonIgnore]
    public string? IssuedRefreshToken { get; set; }

    internal const string HashPrefix = "sha256:";
    internal static string Hash(string value) => HashPrefix + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
