using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.basicDomain.Model.Entity;

[Table("tblposition", Schema = "dbo")]
public class Position : AuditEntity<long>
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("positionid")]
    public long Id { get; set; }

    [Column("factoryid")]
    public int? FactoryId { get; set; }

    [Column("despost")]
    public string? DesPost { get; set; }

    [Column("codpost")]
    public string? CodPost { get; set; }
}
