#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.basicDomain.Model.Entity;

[Table("tblstrategyobject", Schema = "dbo")]
public class StrategyObject
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("strategyobjectid")]
    public long Id { get; set; }

    [Column("mainstrategyid")]
    public int? MainStrategyId { get; set; }

    [Column("code")]
    public string? Code { get; set; }

    [Required, Column("title")]
    public string? Title { get; set; }

    // Compatibility alias: use Title, not this alias, inside EF queries.
    [NotMapped]
    public string? DesStrategyObjectF
    {
        get => Title;
        set => Title = value;
    }

    [Column("weight")]
    public double? Weight { get; set; }

    [Column("themeid")]
    public int? ThemeId { get; set; }

    [Column("originid")]
    public int? OriginId { get; set; }

    [Column("perspectid")]
    public long? PerspectiveId { get; set; }

    [ForeignKey(nameof(PerspectiveId))]
    public Perspective? Perspective { get; set; }

    [Column("levelid")]
    public long? PlaningLevelId { get; set; }

    [ForeignKey(nameof(PlaningLevelId))]
    public PlaningLevel? PlaningLevel { get; set; }

    // References PlanPeriod.PlanId (long), NOT PlanPeriod.Id (int).
    // Apply StrategyObjectConfiguration to establish the principal key.
    [Column("planid")]
    public long? PlanId { get; set; }

    public PlanPeriod? PlanPeriod { get; set; }

    [Column("isdeleted")]
    public bool? Deleted { get; set; } = false;

    // Compatibility alias: use Deleted, not this alias, inside EF queries.
    [NotMapped]
    public bool IsDeleted
    {
        get => Deleted == true;
        set => Deleted = value;
    }
}
