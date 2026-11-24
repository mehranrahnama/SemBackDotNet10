using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.SecurityDomain.Model.Entity;

[Table("tblrevokedtoken", Schema = "dbo")]
public sealed class RevokedToken
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("tokenhash")]
    public string TokenHash { get; set; } = string.Empty;

    [Column("tenantid")]
    public long TenantId { get; set; }

    [Column("expiresat")]
    public DateTimeOffset ExpiresAt { get; set; }

    [Column("revokedat")]
    public DateTimeOffset RevokedAt { get; set; }
}
