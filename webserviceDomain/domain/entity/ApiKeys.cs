using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.Base.Model.Entity;

namespace SemBackDotNet10.webserviceDomain.domain.entity;

[Table("tblapikeys", Schema = "dbo")]
public class ApiKeys : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public override long Id { get; set; }

    [Required]
    [Column("apikey")]
    public string ApiKey { get; set; } = string.Empty;

    [Required]
    [Column("apihash")]
    public string ApiHash { get; set; } = string.Empty;

    [Required]
    [Column("servicename")]
    public string ServiceName { get; set; } = string.Empty;
}
