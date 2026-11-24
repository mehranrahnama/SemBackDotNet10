using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.actionDomain.model.entity;

/// <summary>
/// معادل ActionPlanActivity.java — @Table(name = "tblactionplanactivity", schema = "dbo").
/// وابستگی اعلام‌شده: AuditEntity&lt;TId&gt; (base model entity — هنوز تبدیل نشده)، User (basicDomain).
/// </summary>
[Table("tblactionplanactivity", Schema = "dbo")]
public class ActionPlanActivity : AuditEntity<long>
{
    [Key]
    [Column("actionplanactivityid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>@Column(name = "activityname", nullable = false, length = 500)</summary>
    [Column("activityname")]
    public string ActivityName { get; set; } = null!;

    /// <summary>@Column(name = "datestart", nullable = false, length = 10)</summary>
    [Column("datestart")]
    public string DateStart { get; set; } = null!;

    /// <summary>@Column(name = "dateend", nullable = false, length = 10)</summary>
    [Column("dateend")]
    public string DateEnd { get; set; } = null!;

    /// <summary>@ManyToOne @JoinColumn(name = "actionplancatalogid")</summary>
    public ActionPlanCatalog? ActionPlanCatalog { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "accountable")</summary>
    public User? Accountable { get; set; }

    /// <summary>@Transient — Long accountableId</summary>
    [NotMapped]
    public long? AccountableId { get; set; }

    /// <summary>@Column(name = "weight", nullable = false)</summary>
    [Column("weight")]
    public float? Weight { get; set; }

    /// <summary>@Column(name = "entityid")</summary>
    [Column("entityid")]
    public string? EntityId { get; set; }

    /// <summary>@Transient — Boolean mustBeDeleted، پیش‌فرض جاوا: false</summary>
    [NotMapped]
    public bool? MustBeDeleted { get; set; } = false;

    /// <summary>
    /// معادل Object clone() throws CloneNotSupportedException — اجرای Cloneable در جاوا؛
    /// در C# معادل memberwise clone (شفاف از فیلدهای یکسان) است.
    /// </summary>
    public ActionPlanActivity Clone()
    {
        return (ActionPlanActivity)MemberwiseClone();
    }
}
