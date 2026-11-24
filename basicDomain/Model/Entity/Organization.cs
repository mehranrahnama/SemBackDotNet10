using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.basicDomain.Model.Entity;

[Table("tblorganization", Schema = "dbo")]
public class Organization
{
    [Key]
    [Column("organizationid")]
    public long Id { get; set; }

    [Column("desorganE")]
    [StringLength(100)]
    public string? DesOrganE { get; set; }

    [Column("desorganF")]
    [Required]
    [StringLength(100)]
    public string DesOrganF { get; set; } = string.Empty;

    [Column("desthreechar")]
    [StringLength(9)]
    public string? DesThreeChar { get; set; }

    [Column("parentcode")]
    [StringLength(9)]
    public string? ParentCode { get; set; }

    [Column("tenantid")]
    public long? TenantId { get; set; }

    [ForeignKey(nameof(TenantId))]
    public Tenant? Tenant { get; set; }

    [Column("modirid")]
    public long? ModirId { get; set; }

    public User? Modir { get; set; }
}
