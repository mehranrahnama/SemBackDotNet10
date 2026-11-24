using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.Base.Model.Entity;

namespace SemBackDotNet10.riskDomain.model.entity;

[Table("tblriskaction", Schema = "dbo")]
public class RiskAction : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("riskactionid")]
    public override long Id { get; set; }

    [Column("riskactiontype")]
    public int RiskActionType { get; set; }

    [Column("actionplancatalogid")]
    public long? ActionPlanCatalogId { get; set; }

    [ForeignKey(nameof(ActionPlanCatalogId))]
    public ActionPlanCatalog? ActionPlanCatalog { get; set; }

    [Column("riskactiontitle")]
    public string? RiskActionTitle { get; set; }

    [Column("progresspercent")]
    public double? ProgressPercent { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("riskid")]
    public long? RiskId { get; set; }

    [ForeignKey(nameof(RiskId))]
    public Risk? Risk { get; set; }
}
