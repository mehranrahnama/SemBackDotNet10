using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SemBackDotNet10.Base.Model.Entity;

public abstract class TimeStampEntity<ID> : BaseEntity<ID>
{
    [JsonIgnore]
    [Column("createddate")]
    public DateTime CreatedDate { get; set; }

    [JsonIgnore]
    [Column("modifieddate")]
    public DateTime? ModifiedDate { get; set; }
}
