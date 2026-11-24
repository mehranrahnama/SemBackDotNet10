using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.actionDomain.model.entity;

/// <summary>
/// معادل ActionActPlan.java — @Table(name = "tblactionactplan", schema = "dbo").
/// وابستگی اعلام‌شده: ProcessEntity&lt;TId&gt; (base model entity — هنوز تبدیل نشده)،
/// PlanPeriod و User (basicDomain — هنوز تبدیل نشده).
/// کامنت جاوای حفظ‌شده: // private PlanPeriod planPeriod; (فیلد فعالِ planPeriod در پایین کلاس تعریف شده است)
/// </summary>
[Table("tblactionactplan", Schema = "dbo")]
public class ActionActPlan : ProcessEntity<long>
{
    [Key]
    [Column("actionactplanid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override long Id { get; set; }

    /// <summary>@Column(name = "month", nullable = true)</summary>
    [Column("month")]
    public long? Month { get; set; }

    /// <summary>@Column(name = "planvalue", precision = 4)</summary>
    [Column("planvalue")]
    public decimal? PlanValue { get; set; }

    /// <summary>@Column(name = "actualvalue", precision = 4)</summary>
    [Column("actualvalue")]
    public decimal? ActualValue { get; set; }

    /// <summary>@Column(name = "actualbudget")</summary>
    [Column("actualbudget")]
    public long? ActualBudget { get; set; }

    /// <summary>@Column(name = "temporarydelete")</summary>
    [Column("temporarydelete")]
    public bool? TemporaryDelete { get; set; }

    /// <summary>@Column(name = "dummy_field")</summary>
    [Column("dummy_field")]
    public bool? DummyField { get; set; }

    /// <summary>@Column(name = "deviation", length = 2147483647)</summary>
    [Column("deviation")]
    public string? Deviation { get; set; }

    /// <summary>@Column(name = "correction", length = 2147483647)</summary>
    [Column("correction")]
    public string? Correction { get; set; }

    /// <summary>@Column(name = "correctiondeadlinedate")</summary>
    [Column("correctiondeadlinedate")]
    public DateTime? CorrectionDeadlineDate { get; set; }

    /// <summary>@Column(name = "correctionstartdateaction")</summary>
    [Column("correctionstartdateaction")]
    public DateTime? CorrectionStartDateAction { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "correctionexecuter")</summary>
    public User? CorrectionExecuter { get; set; }

    /// <summary>@Column(name = "correctionactualvalue")</summary>
    [Column("correctionactualvalue")]
    public decimal? CorrectionActualValue { get; set; }

    /// <summary>@Column(name = "evalcomment", length = 2147483647)</summary>
    [Column("evalcomment")]
    public string? EvalComment { get; set; }

    /// <summary>@Column(name = "evalpcn", precision = 4)</summary>
    [Column("evalpcn")]
    public decimal? EvalPcn { get; set; }

    /// <summary>@Column(name = "evaldesc", length = 2147483647)</summary>
    [Column("evaldesc")]
    public string? EvalDesc { get; set; }

    // Version is inherited from BaseEntity<long> (long, ConcurrencyCheck).
    // Do not redeclare a nullable version that hides the concurrency token.

    /// <summary>@Column(name = "taskurl")</summary>
    [Column("taskurl")]
    public string? TaskUrl { get; set; }

    /// <summary>@ManyToOne(fetch = LAZY) @JoinColumn(name = "actionplancatalogid")</summary>
    public ActionPlanCatalog? ActionPlanCatalog { get; set; }

    /*
     * کامنت جاوای حفظ‌شده:
     * @ManyToOne(fetch = FetchType.LAZY)
     * @JoinColumn(name = "planid" ,referencedColumnName = "planid")
     * public PlanPeriod getPlanPeriod() ...
     * (این بلوک در جاوا کامنت شده بود؛ نسخه‌ی فعال planPeriod در پایین کلاس است)
     */

    /// <summary>@Column(name = "Isdeleted") — boolean isDeleted</summary>
    [Column("Isdeleted")]
    public bool IsDeleted { get; set; }

    // OutCome and Confirmstatus are inherited from ProcessEntity<long>.
    // Keeping one backing value also keeps IProcessEntity reads consistent.

    /// <summary>@ManyToOne(fetch = LAZY) @JoinColumn(name = "planid", referencedColumnName = "planid") — نسخه‌ی فعال در جاوا</summary>
    public PlanPeriod? PlanPeriod { get; set; }

    /// <summary>@Column(name = "royalvisionactid")</summary>
    [Column("royalvisionactid")]
    public long? RoyalVisionActId { get; set; }

    /// <summary>@Column(name = "actionpcn", precision = 4)</summary>
    [Column("actionpcn")]
    public decimal? ActionPCN { get; set; }
}
