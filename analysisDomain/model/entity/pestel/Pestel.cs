using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.analysisDomain.model.entity.enums;
using SemBackDotNet10.Base.Model.Entity;

namespace SemBackDotNet10.analysisDomain.model.entity.pestel;

[Table("tblpestel", Schema = "analysis")]
public class Pestel : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("pestelid")]
    public override long Id { get; set; }

    [Column("pestelgroupid")]
    public long PestelGroupId { get; set; }

    [ForeignKey(nameof(PestelGroupId))]
    public PestelGroup PestelGroup { get; set; } = null!;

    [Required]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Column("impact")]
    public Impact? Impact { get; set; }

    [Column("uncertainty")]
    public Uncertainty? Uncertainty { get; set; }
}
