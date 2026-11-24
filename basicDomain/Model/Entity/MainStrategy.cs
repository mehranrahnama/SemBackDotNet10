using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.basicDomain.Model.Entity;

[Table("tblmainstrategy", Schema = "dbo")]
public class MainStrategy
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("mainstrategyid")]
    public long Id { get; set; }

    [Column("desstrategyf")]
    public string? DesStrategyF { get; set; }

    [Column("datestart")]
    public string? DateStart { get; set; }

    [Column("datefinish")]
    public string? DateEnd { get; set; }

    [Column("accountable")]
    public long? AccountableId { get; set; }

    public User? Accountable { get; set; }

    [Column("perspectid")]
    public long? PerspectiveId { get; set; }

    public Perspective? Perspective { get; set; }

    [Column("levelid")]
    public long? PlaningLevelId { get; set; }

    public PlaningLevel? PlaningLevel { get; set; }
}
