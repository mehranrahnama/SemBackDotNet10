using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.Base.Model.Entity;

namespace SemBackDotNet10.analysisDomain.model.entity.porter;

[Table("tblporter", Schema = "analysis")]
public class Porter : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("porterid")]
    public override long Id { get; set; }

    [Column("portergroupid")]
    public long PorterGroupId { get; set; }

    [ForeignKey(nameof(PorterGroupId))]
    public PorterGroup PorterGroup { get; set; } = null!;

    [Required]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Column("nothreat")]
    public string NoThreat { get; set; } = string.Empty;

    [Required]
    [Column("middlethreat")]
    public string MiddleThreat { get; set; } = string.Empty;

    [Required]
    [Column("seriousthreat")]
    public string SeriousThreat { get; set; } = string.Empty;
}
