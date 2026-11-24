using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.Base.Model.Entity;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.riskDomain.model.entity;

[Table("tblrisk", Schema = "dbo")]
public class Risk : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("riskid")]
    public override long Id { get; set; }

    [Column("desrisk")]
    public string? DesRisk { get; set; }

    [Column("cause")]
    public string? Cause { get; set; }

    [Column("effect")]
    public string? Effect { get; set; }

    [Column("severity")]
    public long? Severity { get; set; }

    [Column("probability")]
    public long? Probability { get; set; }

    [Column("strategyobjectid")]
    public long? StrategyObjectId { get; set; }

    [ForeignKey(nameof(StrategyObjectId))]
    public StrategyObject? StrategyObject { get; set; }

    public List<RiskMeasure> RiskMeasures { get; set; } = new();

    public List<RiskAction> RiskActions { get; set; } = new();
}
