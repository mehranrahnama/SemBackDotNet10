#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.Base.Model.Entity;

namespace SemBackDotNet10.basicDomain.Model.Entity;

[Table("tbltenant", Schema = "dbo")]
public class Tenant : AuditEntity<long>
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("tenantid")]
    public override long Id { get; set; }

    [MaxLength(500), Column("tenantname")]
    public string? TenantName { get; set; }

    [MaxLength(10), Column("tenantcode")]
    public string? TenantCode { get; set; }

    [Column("tenantparentid")]
    public long? TenantParentId { get; set; }

    [ForeignKey(nameof(TenantParentId))]
    public virtual Tenant? Parent { get; set; }

    // Compatibility with the previous minimal C# model; not a second column.
    [NotMapped]
    public string? Name
    {
        get => TenantName;
        set => TenantName = value;
    }
}
