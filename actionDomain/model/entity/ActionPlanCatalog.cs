using SemBackDotNet10.Base.Model.Entity;
using SemBackDotNet10.basicDomain.Model.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.actionDomain.model.entity;

/// <summary>
/// معادل ActionPlanCatalog.java — @Table(name = "tblactionplancatalog", schema = "dbo").
/// وابستگی اعلام‌شده: ProcessEntity&lt;TId&gt; (base model entity — هنوز تبدیل نشده)،
/// PlaningLevel و User (basicDomain — هنوز تبدیل نشده).
/// </summary>
[Table("tblactionplancatalog", Schema = "dbo")]
public class ActionPlanCatalog : ProcessEntity<long>
{
    [Key]
    [Column("actionplancatalogid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override long Id { get; set; }

    /// <summary>@Basic @Nationalized @Column(name = "description", nullable = false, length = 200)</summary>
    [Column("description")]
    public string Description { get; set; } = null!;

    /// <summary>@Column(name = "startdate", nullable = false, length = 10)</summary>
    [Column("startdate")]
    public string StartDate { get; set; } = null!;

    /// <summary>@Column(name = "enddate", nullable = false, length = 10)</summary>
    [Column("enddate")]
    public string EndDate { get; set; } = null!;

    /// <summary>@Column(name = "totalbudget")</summary>
    [Column("totalbudget")]
    public int? TotalBudget { get; set; }

    /// <summary>@Column(name = "code", length = 20)</summary>
    [Column("code")]
    public string? Code { get; set; }

    /// <summary>@Column(name = "originid")</summary>
    [Column("originid")]
    public int? OriginId { get; set; }

    // Confirmstatus is mapped by ProcessEntity<long>.

    /// <summary>@ManyToOne @JoinColumn(name = "levelid")</summary>
    public PlaningLevel? PlaningLevel { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "payeshgar", referencedColumnName = "userid")</summary>
    public User? Payeshgar { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "modir", referencedColumnName = "userid")</summary>
    public User? Modir { get; set; }

    /// <summary>@Transient — Boolean addToSupremeLevel، پیش‌فرض جاوا: false</summary>
    [NotMapped]
    public bool? AddToSupremeLevel { get; set; } = false;

    /// <summary>@Column(name = "isdeleted") — Boolean deleteStatus، پیش‌فرض جاوا: false</summary>
    [Column("isdeleted")]
    public bool? DeleteStatus { get; set; } = false;

    /// <summary>@Column(name = "taskurl")</summary>
    [Column("taskurl")]
    public string? TaskUrl { get; set; }

    /// <summary>@OneToMany(mappedBy = "actionPlanCatalog", targetEntity = ActionPlan.class)</summary>
    public List<ActionPlan>? ActionPlans { get; set; }

    /// <summary>@OneToMany(mappedBy = "actionPlanCatalog", targetEntity = ActionActPlan.class)</summary>
    public List<ActionActPlan>? ActionActPlans { get; set; }

    /// <summary>@Column(name = "isplanned", nullable = false) — boolean isPlanned</summary>
    [Column("isplanned")]
    public bool IsPlanned { get; set; }

    /// <summary>@Column(name = "replannedstatus")</summary>
    [Column("replannedstatus")]
    public long? ReplannedStatus { get; set; }

    /// <summary>@Column(name = "isreplanned", nullable = false) — boolean isReplanned</summary>
    [Column("isreplanned")]
    public bool IsReplanned { get; set; }

    /// <summary>@Column(name = "caseinstanceid")</summary>
    [Column("caseinstanceid")]
    public string? CaseInstanceId { get; set; }

    /// <summary>@Column(name = "businesskey")</summary>
    [Column("businesskey")]
    public string? BusinessKey { get; set; }

    /// <summary>@Column(name = "processid")</summary>
    [Column("processid")]
    public string? ProcessId { get; set; }

    /// <summary>@Column(name = "definitionid")</summary>
    [Column("definitionid")]
    public string? DefinitionId { get; set; }
}
