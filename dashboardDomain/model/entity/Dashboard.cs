using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.Base.Model.Entity;

namespace SemBackDotNet10.dashboardDomain.model.entity;

/// <summary>
/// جدول dbo.tbldashboard. به‌جای ManyToOne روی Measure/ActionPlan/User (که در پروژه چند کلاس هم‌نام دارند)
/// فقط کلید خارجی نگه‌داری می‌شود.
/// </summary>
[Table("tbldashboard", Schema = "dbo")]
public class Dashboard : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("dashbordid")]
    public override long Id { get; set; }

    [Column("userid")]
    public long? UserId { get; set; }

    [Column("dimmeasureid")]
    public long? MeasureId { get; set; }

    [Column("actionplanid")]
    public long? ActionPlanId { get; set; }

    [Column("isdeleted")]
    public bool Deleted { get; set; }
}
