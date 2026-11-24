using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.basicDomain.Model.Entity;

[Table("tblperspective", Schema = "dbo")]
public class Perspective
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("perspectid")]
    public long Id { get; set; }

    [Column("desperspectE")]
    [StringLength(200)]
    public string? DesperspectE { get; set; }

    [NotMapped]
    public long? PlanId { get; set; }

    [Column("desperspectF")]
    [Required]
    [StringLength(200)]
    public string DesperspectF { get; set; } = string.Empty;

    [Column("weight")]
    public int? Weight { get; set; }

    [Column("orders")]
    public long? Orders { get; set; }

    [Column("parentid")]
    public long? ParentId { get; set; }

    [Column("rank")]
    public long? Rank { get; set; }

    public Perspective? Parent { get; set; }

    [Column("levelid")]
    public long? PlaningLevelId { get; set; }

    [Column("code")]
    [StringLength(20)]
    public string? Code { get; set; }

    [Column("originid")]
    public long? OriginId { get; set; }

    public PlaningLevel? PlaningLevel { get; set; }

    [Column("planid")]
    public long? PlanPeriodId { get; set; }

    public PlanPeriod? PlanPeriod { get; set; }
}
