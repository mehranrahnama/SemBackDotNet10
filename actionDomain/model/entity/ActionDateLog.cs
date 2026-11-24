using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.actionDomain.model.entity;

/// <summary>
/// معادل ActionDateLog.java — @Table(name = "tblactiondatelog", schema = "dbo").
/// وابستگی اعلام‌شده: AuditEntity&lt;TId&gt; (base model entity — هنوز تبدیل نشده).
/// </summary>
[Table("tblactiondatelog", Schema = "dbo")]
public class ActionDateLog : AuditEntity<long>
{
    [Key]
    [Column("actiondatelogid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>@Column(name = "enddateold", length = 10)</summary>
    [Column("enddateold")]
    public string? EndDateOld { get; set; }

    /// <summary>@Column(name = "enddatenew", length = 10)</summary>
    [Column("enddatenew")]
    public string? EndDateNew { get; set; }

    /// <summary>@Column(name = "reasonenddatechange", length = 1000)</summary>
    [Column("reasonenddatechange")]
    public string? ReasonEndDateChange { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "actionplancatalogid", referencedColumnName = "actionplancatalogid")</summary>
    public ActionPlanCatalog? ActionPlanCatalog { get; set; }
}
