using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.Base.Model.Entity;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.analysisDomain.model.entity.porter;

[Table("tblportergroup", Schema = "analysis")]
public class PorterGroup : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("portergroupid")]
    public override long Id { get; set; }

    [Required]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("planid")]
    public long? PlanId { get; set; }

    [ForeignKey(nameof(PlanId))]
    public PlanPeriod? PlanPeriod { get; set; }
}
