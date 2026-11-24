using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.analysisDomain.model.entity.enums;
using SemBackDotNet10.Base.Model.Entity;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.analysisDomain.model.entity.porter;

[Table("tblporterresult", Schema = "analysis")]
public class PorterResult : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("porterresultid")]
    public override long Id { get; set; }

    [Column("portergroupid")]
    public long PorterGroupId { get; set; }

    [ForeignKey(nameof(PorterGroupId))]
    public PorterGroup PorterGroup { get; set; } = null!;

    [Column("planid")]
    public long? PlanId { get; set; }

    [ForeignKey(nameof(PlanId))]
    public PlanPeriod? PlanPeriod { get; set; }

    [Column("productid")]
    public long ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;

    [Column("marketid")]
    public long MarketId { get; set; }

    [ForeignKey(nameof(MarketId))]
    public Market Market { get; set; } = null!;

    [Column("result")]
    public Threat? Result { get; set; }

    [Required]
    [Column("description")]
    public string Description { get; set; } = string.Empty;
}
