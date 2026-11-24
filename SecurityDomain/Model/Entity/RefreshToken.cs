using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.SecurityDomain.Model.Entity;



[Table("tblrefreshtoken", Schema = "dbo")]
public sealed class RefreshToken
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("tenantid")]
    public long TenantId { get; set; }

    [Column("accountid")]
    public long AccountId { get; set; }

    [Required]
    [Column("familyid")]
    public Guid FamilyId { get; set; }

    [Required]
    [Column("tokenhash")]
    public string TokenHash { get; set; } = string.Empty;

    [Column("expiresat")]
    public DateTimeOffset ExpiresAt { get; set; }

    [Column("revokedat")]
    public DateTimeOffset? RevokedAt { get; set; }

    [Column("consumedat")]
    public DateTimeOffset? ConsumedAt { get; set; }

    [Column("replacedbyhash")]
    public string? ReplacedByHash { get; set; }
}