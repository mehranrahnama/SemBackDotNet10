// C# migration of model/entity/AuditableEntity.java

namespace Common.model.entity;

// External navigation types represented by JsonObject: User
// Replace with real domain entities before ORM integration.
public class AuditableEntity
{
    [JsonPropertyName("createdBy")]
    public System.Text.Json.Nodes.JsonObject? CreatedBy { get; set; }
    [JsonPropertyName("modifiedBy")]
    public System.Text.Json.Nodes.JsonObject? ModifiedBy { get; set; }
    [Column("created_date")] [JsonPropertyName("createdDate")]
    public DateTime? CreatedDate { get; set; }
    [Column("modified_date")] [JsonPropertyName("modifiedDate")]
    public DateTime? ModifiedDate { get; set; }
    [Column("version")] [ConcurrencyCheck] [JsonPropertyName("version")]
    public long? Version { get; set; }
}
