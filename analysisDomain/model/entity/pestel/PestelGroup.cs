using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.Base.Model.Entity;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.analysisDomain.model.entity.pestel;

[Table("tblpestelgroup", Schema = "analysis")]
public class PestelGroup : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("pestelgroupid")]
    public override long Id { get; set; }

    [Required]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("planid")]
    public long? PlanId { get; set; }

    [ForeignKey(nameof(PlanId))]
    public PlanPeriod? PlanPeriod { get; set; }
}
