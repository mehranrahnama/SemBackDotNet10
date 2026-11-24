using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.SecurityDomain.Model.Entity;

/// <summary>
/// معادل ControlPanel.java — جدول dbo.tblcontrolpanel.
/// وابستگی‌های اعلام‌شده: AuditEntity&lt;TId&gt; (base model — هنوز تبدیل نشده)،
/// PlaningLevel (basicDomain — هنوز تبدیل نشده)، User (basicDomain — هنوز تبدیل نشده).
/// </summary>
[Table("tblcontrolpanel", Schema = "dbo")]
public class ControlPanel : AuditEntity<long>
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override long Id { get; set; }

    [Column("entitytype")]
    public int? Entitytype { get; set; }

    [Column("regdate_start")]
    public string Regdatestart { get; set; } = null!;

    [Column("regdate_end")]
    public string Regdateend { get; set; } = null!;

    [Column("valdate_start")]
    public string ValdateStart { get; set; } = null!;

    [Column("valdate_end")]
    public string Valdateend { get; set; } = null!;

    [Column("month_val_spec")]
    public string Monthvalspec { get; set; } = null!;

    [Column("year_val_spec")]
    public string Yearvalspec { get; set; } = null!;

    [Column("isalllevelsallowed_spec")]
    public bool? Alllevelsallowedspec { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "levelidallowed_spec")</summary>
    [ForeignKey(nameof(PlaningLevel))]
    [Column("levelidallowed_spec")]
    public PlaningLevel? PlaningLevel { get; set; }

    [Column("isalltashilgarsallowed_spec")]
    public bool? Alltashilgarsallowedspec { get; set; }

    /// <summary>@ManyToOne @JoinColumn(name = "tashilgaridallowed_spec")</summary>
    [ForeignKey(nameof(Tashilgar))]
    [Column("tashilgaridallowed_spec")]
    public User? Tashilgar { get; set; }

    [Column("val_month")]
    public string? Val_month { get; set; }

    [Column("allowedplaninglevelids")]
    public string AllowedPlaningLevelIds { get; set; } = null!;
}
