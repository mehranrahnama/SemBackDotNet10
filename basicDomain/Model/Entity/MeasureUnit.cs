using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.basicDomain.Model.Entity;

[Table("tblmeasureunit", Schema = "dbo")]
public class MeasureUnit
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("measureunitid")]
    public long Id { get; set; }

    [Column("desmeasuree")]
    public string? DesMeasureE { get; set; }

    [Column("desmeasuref")]
    [Required]
    public string DesMeasureF { get; set; } = string.Empty;

    [Column("iscumulative")]
    public bool? Cumulative { get; set; }

    [Column("code")]
    public string? Code { get; set; }
}
