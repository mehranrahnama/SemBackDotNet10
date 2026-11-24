using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.actionDomain.model.entity;

/// <summary>
/// معادل ActionActPlanLog.java — @Table(name = "tblactionactplanlog", schema = "dbo").
/// وابستگی اعلام‌شده: AuditEntity&lt;TId&gt; (base model entity — هنوز تبدیل نشده)،
/// User و PlanPeriod (basicDomain — هنوز تبدیل نشده).
/// </summary>
[Table("tblactionactplanlog", Schema = "dbo")]
public class ActionActPlanLog : AuditEntity<long>
{
    [Key]
    [Column("actionactplanlogid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override long Id { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "userid", referencedColumnName = "userid")</summary>
    public User? User { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "planid", referencedColumnName = "planid")</summary>
    public PlanPeriod? PlanPeriod { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "actionplancatalogid", referencedColumnName = "actionplancatalogid")</summary>
    [Column("actionplancatalogid")]
    public long? ActionPlanCatalogId { get; set; }

    [ForeignKey(nameof(ActionPlanCatalogId))]
    public ActionPlanCatalog? ActionPlanCatalog { get; set; }


    /// <summary>@Column(name = "changedate", length = 10)</summary>
    [Column("changedate")]
    public string? ChangeDate { get; set; }

    /// <summary>@Column(name = "month")</summary>
    [Column("month")]
    public int? Month { get; set; }

    /// <summary>@Column(name = "planvalueold", precision = 4)</summary>
    [Column("planvalueold")]
    public decimal? PlanValueOld { get; set; }

    /// <summary>@Column(name = "planvaluenew", precision = 4)</summary>
    [Column("planvaluenew")]
    public decimal? PlanValueNew { get; set; }

    /// <summary>@Column(name = "actualvalueold", precision = 4)</summary>
    [Column("actualvalueold")]
    public decimal? ActualValueOld { get; set; }

    /// <summary>@Column(name = "actualvaluenew", precision = 4)</summary>
    [Column("actualvaluenew")]
    public decimal? ActualValueNew { get; set; }
}
