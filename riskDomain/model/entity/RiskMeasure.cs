using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.Base.Model.Entity;
using SemBackDotNet10.measureDomain.model.entity;

namespace SemBackDotNet10.riskDomain.model.entity;

[Table("tblriskmeasure", Schema = "dbo")]
public class RiskMeasure : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("riskmeasureid")]
    public override long Id { get; set; }

    [Column("measurecatalogid")]
    public long? MeasureCatalogId { get; set; }

    [ForeignKey(nameof(MeasureCatalogId))]
    public MeasureCatalog? MeasureCatalog { get; set; }

    [Column("riskmeasuretitle")]
    public string? RiskMeasureTitle { get; set; }

    [Column("riskid")]
    public long? RiskId { get; set; }

    [ForeignKey(nameof(RiskId))]
    public Risk? Risk { get; set; }

    /// <summary>Only one of measureCatalog or riskMeasureTitle must have value.</summary>
    [NotMapped]
    public bool IsValid =>
        ((MeasureCatalog != null || MeasureCatalogId != null) && RiskMeasureTitle == null)
        || (MeasureCatalog == null && MeasureCatalogId == null && RiskMeasureTitle != null);
}
