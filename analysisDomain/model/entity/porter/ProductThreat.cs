using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.analysisDomain.model.entity.enums;
using SemBackDotNet10.Base.Model.Entity;

namespace SemBackDotNet10.analysisDomain.model.entity.porter;

[Table("tblproductthreat", Schema = "analysis")]
public class ProductThreat : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("productthreatid")]
    public override long Id { get; set; }

    [Column("productid")]
    public long ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;

    [Column("marketid")]
    public long MarketId { get; set; }

    [ForeignKey(nameof(MarketId))]
    public Market Market { get; set; } = null!;

    [Column("threat")]
    public Threat? Threat { get; set; }

    [Column("porterid")]
    public long PorterId { get; set; }

    [ForeignKey(nameof(PorterId))]
    public Porter Porter { get; set; } = null!;
}
