using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.Base.Model.Entity;

namespace SemBackDotNet10.analysisDomain.model.entity.porter;

[Table("tblproduct", Schema = "analysis")]
public class Product : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("productid")]
    public override long Id { get; set; }

    [Column("productgroupid")]
    public long ProductGroupId { get; set; }

    [ForeignKey(nameof(ProductGroupId))]
    public ProductGroup ProductGroup { get; set; } = null!;

    [Required]
    [Column("title")]
    public string Title { get; set; } = string.Empty;
}
