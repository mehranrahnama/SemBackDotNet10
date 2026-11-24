using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.Base.Model.Entity;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.analysisDomain.model.entity.resource;

[Table("tblresource", Schema = "analysis")]
public class Resource : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("resourceid")]
    public override long Id { get; set; }

    [Required]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("levelid")]
    public long? PlaningLevelId { get; set; }

    [ForeignKey(nameof(PlaningLevelId))]
    public PlaningLevel? PlaningLevel { get; set; }

    // Java property "resource" (a member cannot share its enclosing type's name in C#)
    [Column("resource")]
    public bool? IsResource { get; set; }

    [Column("existancestatus")]
    public bool ExistenceStatus { get; set; }

    [Column("planid")]
    public long? PlanId { get; set; }

    [ForeignKey(nameof(PlanId))]
    public PlanPeriod? PlanPeriod { get; set; }

    [Column("power")]
    public long Power { get; set; }

    [Column("importance")]
    public long Importance { get; set; }

    [Column("valuable")]
    public bool? Valuable { get; set; }

    [Column("rare")]
    public bool? Rare { get; set; }

    [Column("costlyinitiate")]
    public bool? CostlyInitiate { get; set; }

    [Column("organize")]
    public bool? Organize { get; set; }

    [Column("competencyresult")]
    public string? CompetencyResult { get; set; }
}
