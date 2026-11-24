using SemBackDotNet10.Base.Model.Entity;
using SemBackDotNet10.basicDomain.Model.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.actionDomain.model.entity;

/// <summary>
/// معادل ActionPlan.java — @Table(name = "tblactionplan", schema = "dbo").
/// وابستگی اعلام‌شده: ProcessEntity&lt;TId&gt; (base model entity — هنوز تبدیل نشده)،
/// PlanPeriod، PlaningLevel، StrategyObject و User (basicDomain — هنوز تبدیل نشده).
/// </summary>
[Table("tblactionplan", Schema = "dbo")]
public class ActionPlan : ProcessEntity<long>
{
    [Key]
    [Column("actionplanid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override long Id { get; set; }

    /// <summary>@Column(name = "weight", nullable = false, precision = 4)</summary>
    [Column("weight")]
    public float? Weight { get; set; }

    /// <summary>@Column(name = "priority", nullable = false)</summary>
    [Column("priority")]
    public int Priority { get; set; }

    /// <summary>@Column(name = "yearstartpcn")</summary>
    [Column("yearstartpcn")]
    public decimal? YearStartPcn { get; set; }

    /// <summary>@Column(name = "yearcompletepcn")</summary>
    [Column("yearcompletepcn")]
    public decimal? YearCompletePcn { get; set; }

    /// <summary>@Column(name = "yearbudget")</summary>
    [Column("yearbudget")]
    public int? YearBudget { get; set; }

    /// <summary>@Column(name = "type", nullable = false)</summary>
    [Column("type")]
    public long Type { get; set; }

    /// <summary>@Column(name = "dummy_update")</summary>
    [Column("dummy_update")]
    public bool? DummyUpdate { get; set; }

    /// <summary>'@Column(name = "sendaction")'</summary>
    [Column("sendaction")]
    public bool? SendAction { get; set; }

    /// <summary>@Column(name = "datesend", length = 50)</summary>
    [Column("datesend")]
    public string? DateSend { get; set; }

    /// <summary>@Column(name = "confirmstatus")</summary>
    [Column("confirmstatus")]
    public long? Confirmstatus { get; set; }

    /// <summary>@Column(name = "taskid", length = 500)</summary>
    [Column("taskid")]
    public string? TaskId { get; set; }

    /// <summary>@Column(name = "taskurl")</summary>
    [Column("taskurl")]
    public string? TaskUrl { get; set; }

    /// <summary>@Column(name = "isdeleted") — Boolean deleteStatus، پیش‌فرض جاوا: false</summary>
    [Column("isdeleted")]
    public bool? DeleteStatus { get; set; } = false;

    /// <summary>@ManyToOne @JoinColumn(name = "actionplancatalogid")</summary>
    public ActionPlanCatalog? ActionPlanCatalog { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "strategyobjectid")</summary>
    public StrategyObject? StrategyObject { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "planid", referencedColumnName = "planid")</summary>
    public PlanPeriod? PlanPeriod { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "levelid")</summary>
    public PlaningLevel? PlaningLevel { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "modir")</summary>
    public User? Modir { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "payeshgar")</summary>
    public User? Payeshgar { get; set; }

    /// <summary>@Transient — Long actionPlanCatalogId</summary>
    [NotMapped]
    public long? ActionPlanCatalogId { get; set; }

    /// <summary>@Transient — Long strategyObjectId</summary>
    [NotMapped]
    public long? StrategyObjectId { get; set; }

    /// <summary>@Transient — Long planPeriodId</summary>
    [NotMapped]
    public long? PlanPeriodId { get; set; }

    /// <summary>@Transient — Long planingLevelId</summary>
    [NotMapped]
    public long? PlaningLevelId { get; set; }

    /// <summary>@Transient — Long modirId</summary>
    [NotMapped]
    public long? ModirId { get; set; }

    /// <summary>@Transient — Long payeshgarId</summary>
    [NotMapped]
    public long? PayeshgarId { get; set; }

    /// <summary>@Column(name = "addtosupremelevel") — boolean، پیش‌فرض جاوا: false</summary>
    [Column("addtosupremelevel")]
    public bool AddToSupremeLevel { get; set; } = false;

    /// <summary>@Column(name = "sendtoparentonly", nullable = false) — boolean، پیش‌فرض جاوا: false</summary>
    [Column("sendtoparentonly")]
    public bool SendToParentOnly { get; set; } = false;

    /// <summary>@Transient — List&lt;Long&gt; objectIdList</summary>
    [NotMapped]
    public List<long>? ObjectIdList { get; set; }

    /// <summary>@Transient — String outCome</summary>
    [NotMapped]
    public string? OutCome { get; set; }

    /// <summary>@Transient — List&lt;Long&gt; beneficiariesId</summary>
    [NotMapped]
    public List<long>? BeneficiariesId { get; set; }

    /// <summary>@Transient — List&lt;Long&gt; coWorkersId</summary>
    [NotMapped]
    public List<long>? CoWorkersId { get; set; }
}
