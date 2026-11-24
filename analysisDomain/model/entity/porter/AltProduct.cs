using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.Base.Model.Entity;

namespace SemBackDotNet10.analysisDomain.model.entity.porter;

[Table("tblaltproduct", Schema = "analysis")]
public class AltProduct : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("altproductid")]
    public override long Id { get; set; }

    [Column("productid")]
    public long ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;

    [Column("title")]
    public string? Title { get; set; }
}
