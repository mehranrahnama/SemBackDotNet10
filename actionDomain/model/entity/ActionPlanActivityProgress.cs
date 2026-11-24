using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.actionDomain.model.entity;

/// <summary>
/// معادل ActionPlanActivityProgress.java — @Table(name = "tblactionplanactivityprogress", schema = "dbo").
/// وابستگی اعلام‌شده: AuditEntity&lt;TId&gt; (base model entity — هنوز تبدیل نشده)، PlanPeriod (basicDomain).
/// lombok @Setter در C# نیازی ندارد.
/// </summary>
[Table("tblactionplanactivityprogress", Schema = "dbo")]
public class ActionPlanActivityProgress : AuditEntity<long>
{
    [Key]
    [Column("actionplanactivityprogressid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "actionplanactivityid")</summary>
    public ActionPlanActivity? ActionPlanActivity { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "planid", referencedColumnName = "planid")</summary>
    public PlanPeriod? PlanPeriod { get; set; }

    /// <summary>@Column(name = "month")</summary>
    [Column("month")]
    public long? Month { get; set; }

    /// <summary>@Column(name = "actualvalue")</summary>
    [Column("actualvalue")]
    public decimal? ActualValue { get; set; }

    public ActionPlanActivityProgress() { }
}
