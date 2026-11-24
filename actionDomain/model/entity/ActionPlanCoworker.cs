using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.actionDomain.model.entity;

/// <summary>
/// معادل ActionPlanCoworker.java — @Table(name = "tblactionplancoworker", schema = "dbo").
/// وابستگی اعلام‌شده: PlaningLevelOrganization (basicDomain — هنوز تبدیل نشده).
/// </summary>
[Table("tblactionplancoworker", Schema = "dbo")]
public class ActionPlanCoworker
{
    [Key]
    [Column("actionplancoworkerid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "actionplanid")</summary>
    public ActionPlan? ActionPlan { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "planinglevelorgid")</summary>
    public PlaningLevelOrganization? PlaningLevelOrganization { get; set; }
}
