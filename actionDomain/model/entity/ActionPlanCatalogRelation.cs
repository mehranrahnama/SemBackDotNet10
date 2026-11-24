using SemBackDotNet10.Base.Model.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.actionDomain.model.entity;

/// <summary>
/// معادل ActionPlanCatalogRelation.java — @Table(name = "tblactionplancatalogrelation", schema = "dbo").
/// وابستگی اعلام‌شده: AuditEntity&lt;TId&gt; (base model entity — هنوز تبدیل نشده).
/// کامنت جاوای حفظ‌شده: فیلد PlanPeriod و getter/setter آن کامنت شده بودند.
/// </summary>
[Table("tblactionplancatalogrelation", Schema = "dbo")]
public class ActionPlanCatalogRelation : AuditEntity<long>
{
    [Key]
    [Column("actionplancatrelationid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override long Id { get; set; }

    /// <summary>@Column(name = "weight", nullable = false, precision = 1) — پیش‌فرض جاوا: BigDecimal.ZERO</summary>
    [Column("weight")]
    public decimal Weight { get; set; } = decimal.Zero;

    /// <summary>@ManyToOne @JoinColumn(name = "mainactionplancatalogid")</summary>
    public ActionPlanCatalog? MainActionPlanCatalog { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "relatedactionplancatalogid", nullable = false)</summary>
    public ActionPlanCatalog? RelatedActionPlanCatalog { get; set; }
}
