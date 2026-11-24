using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.actionDomain.model.entity;

/// <summary>
/// معادل ActionPlanBeneficiary.java — @Table(name = "tblactionplanbeneficiary", schema = "dbo").
/// وابستگی اعلام‌شده: PlaningLevelOrganization (basicDomain — هنوز تبدیل نشده).
/// </summary>
[Table("tblactionplanbeneficiary", Schema = "dbo")]
public class ActionPlanBeneficiary
{
    [Key]
    [Column("actionplanbeneficiaryid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "actionplanid")</summary>
    public ActionPlan? ActionPlan { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "planinglevelorgid")</summary>
    public PlaningLevelOrganization? PlaningLevelOrganization { get; set; }
}
