// C# migration of model/entity/History.java

namespace Common.model.entity;

[Table("tblhistory", Schema = "dbo")]
public class History
{
    [Column("historyid")] [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [Column("entityname")] [JsonPropertyName("entityName")]
    public string? EntityName { get; set; }
    [Column("entityid")] [JsonPropertyName("entityId")]
    public long? EntityId { get; set; }
    [Column("status")] [JsonPropertyName("status")]
    public long? Status { get; set; }
    [Column("createddate")] [JsonIgnore]
    public DateTime? CreatedDate { get; set; }
    [Column("modifieddate")] [JsonIgnore]
    public DateTime? ModifiedDate { get; set; }
    [Column("createdby")] [JsonIgnore]
    public long? CreatedById { get; set; }
    [Column("modifiedby")] [JsonIgnore]
    public long? ModifiedById { get; set; }
    [NotMapped] [JsonPropertyName("userId")]
    public long? UserId { get; set; }
    [NotMapped] [JsonPropertyName("creatorDisplayName")]
    public string? CreatorDisplayName { get; set; }
    [NotMapped] [JsonPropertyName("creationDate")]
    public DateTime? CreationDate { get; set; }
    [Column("Outcome")] [JsonPropertyName("outCome")]
    public string? OutCome { get; set; }
    [Column("Deletefirststatus")] [JsonPropertyName("DeleteFirstStatus")]
    public long? DeleteFirstStatus { get; set; }
}
