using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.Base.Model.Entity;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.analysisDomain.model.entity.resource;

[Table("tblresourcecompetencyrule", Schema = "analysis")]
public class ResourceCompetencyRule : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("resourcecompetencyruleid")]
    public override long Id { get; set; }

    [Required]
    [Column("valuev")]
    public string ValueV { get; set; } = string.Empty;

    [Required]
    [Column("resultv")]
    public string ResultV { get; set; } = string.Empty;

    [Required]
    [Column("valuer")]
    public string ValueR { get; set; } = string.Empty;

    [Required]
    [Column("resultr")]
    public string ResultR { get; set; } = string.Empty;

    [Required]
    [Column("valuei")]
    public string ValueI { get; set; } = string.Empty;

    [Required]
    [Column("resulti")]
    public string ResultI { get; set; } = string.Empty;

    [Required]
    [Column("valueo")]
    public string ValueO { get; set; } = string.Empty;

    [Required]
    [Column("resulto")]
    public string ResultO { get; set; } = string.Empty;

    [Required]
    [Column("finalresult")]
    public string FinalResult { get; set; } = string.Empty;

    [Column("planid")]
    public long? PlanId { get; set; }

    [ForeignKey(nameof(PlanId))]
    public PlanPeriod? PlanPeriod { get; set; }
}
