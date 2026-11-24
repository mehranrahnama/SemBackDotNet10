using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.basicDomain.Model.Entity;

[Table("tblplaninglevel", Schema = "dbo")]
public class PlaningLevel : AuditEntity<long>
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("levelid")]
    public long Id { get; set; }

    [Required, Column("tenantid")]
    public int FactoryId { get; set; } = 1;

    [Column("code"), StringLength(20)]
    public string? Code { get; set; }

    [Required, Column("deslevel"), StringLength(150)]
    public string DesLevel { get; set; } = string.Empty;

    [Column("desthreechar")]
    public string? DesThreeChar { get; set; }

    [Required, Column("typelevel")]
    public short TypeLevel { get; set; }

    [Column("createdate")]
    public string? CreateDate { get; set; }

    [Column("parentid")]
    public long? ParentId { get; set; }

    [ForeignKey(nameof(ParentId))]
    public virtual PlaningLevel? Parent { get; set; }

    [Column("responsibleid")]
    public long? ResponsibleId { get; set; }

    [ForeignKey(nameof(ResponsibleId))]
    public virtual User? Responsible { get; set; }

    [Column("facilitator")]
    public long? FacilitatorId { get; set; }

    [ForeignKey(nameof(FacilitatorId))]
    public virtual User? Facilitator { get; set; }

    [Column("modir")]
    public long? ModirId { get; set; }

    [ForeignKey(nameof(ModirId))]
    public virtual User? Modir { get; set; }

    [Column("tashilgar")]
    public long? TashilgarId { get; set; }

    [ForeignKey(nameof(TashilgarId))]
    public virtual User? Tashilgar { get; set; }

    [Column("operational")]
    public bool? Operational { get; set; }

    public virtual ICollection<PlaningLevelUser> PlaningLevelUsers { get; set; } = new List<PlaningLevelUser>();
}
