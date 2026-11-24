using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.actionDomain.model.entity;

/// <summary>
/// معادل ActionPlanCatalogLog.java — @Table(name = "tblactionplancataloglog", schema = "dbo").
/// وابستگی اعلام‌شده: AuditEntity&lt;TId&gt; (base model entity — هنوز تبدیل نشده)، User (basicDomain).
/// lombok @Builder/@Getter/@NoArgsConstructor/@AllArgsConstructor در C# نیازی ندارد؛
/// توضیح خاص @Getter روی فیلد actionPlanCatalog: در C# همان property عمومی است.
/// </summary>
[Table("tblactionplancataloglog", Schema = "dbo")]
public class ActionPlanCatalogLog : AuditEntity<long>
{
    [Key]
    [Column("actionplanlogid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "actionplancatalogid") — روی این فیلد در جاوا @Getter بود</summary>
    public ActionPlanCatalog? ActionPlanCatalog { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "oldpayeshgarid")</summary>
    public User? OldPayeshgar { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "newpayeshgarid")</summary>
    public User? NewPayeshgar { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "oldmodirid")</summary>
    public User? OldModir { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "newmodirid")</summary>
    public User? NewModir { get; set; }
}
