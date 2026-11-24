using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace SemBackDotNet10.Base.Model.Entity;

public abstract class AuditEntity<ID> : TimeStampEntity<ID>
{
    // Foreign Keys
    public long? CreatedById { get; set; }
    public long? ModifiedById { get; set; }

    // Navigation properties
    [JsonIgnore]
    [ForeignKey(nameof(CreatedById))]
    public virtual SecurityDomain.Model.Entity.User? CreatedBy { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(ModifiedById))]
    public virtual SecurityDomain.Model.Entity.User? ModifiedBy { get; set; }
}
