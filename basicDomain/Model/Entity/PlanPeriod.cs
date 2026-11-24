using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.basicDomain.Model.Entity;

[Table("tblplanperiod", Schema = "dbo")]
public class PlanPeriod
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("planperiodid", TypeName = "int")]
    public int Id { get; set; }

    [Column("createddate")]
    public DateTime? CreatedDate { get; set; }

    [Column("modifieddate")]
    public DateTime? ModifiedDate { get; set; }

    [Column("version")]
    public long? Version { get; set; }

    [Column("createdby")]
    public long? CreatedById { get; set; }

    [Column("modifiedby")]
    public long? ModifiedById { get; set; }

    [Required, Column("planid")]
    public long PlanId { get; set; }

    [Required, Column("statustype")]
    public short StatusType { get; set; }

    public static string? GetStatusTypeDescription(short statusType) => statusType switch
    {
        2 => "فعال",
        3 => "مختومه",
        _ => null
    };
}
