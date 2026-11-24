// C# migration of model/entity/Comment.java

namespace Common.model.entity;

[Table("tblcomment", Schema = "dbo")]
public class Comment
{
    // Inherited TimestampEntity field, discovered in the full Java source.
    [Column("createddate")] [JsonIgnore]
    public DateTime? CreatedDate { get; set; }
    [Column("modifieddate")] [JsonIgnore]
    public DateTime? ModifiedDate { get; set; }
    [Column("createdby")] [JsonIgnore]
    public long? CreatedById { get; set; }
    [Column("modifiedby")] [JsonIgnore]
    public long? ModifiedById { get; set; }
    [Column("Commentid")] [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [Column("Entityname")] [JsonPropertyName("entityName")]
    public string? EntityName { get; set; }
    [Column("Entityid")] [JsonPropertyName("entityId")]
    public long? EntityId { get; set; }
    [Column("Status")] [JsonPropertyName("status")]
    public long? Status { get; set; }
    [Column("Commentdes")] [JsonPropertyName("commentDes")]
    public string? CommentDes { get; set; }
    [Column("Lock")] [JsonPropertyName("lock")]
    public bool Lock { get; set; }
    [NotMapped] [JsonPropertyName("commentDate")]
    public DateTime? CommentDate { get; set; }
    [NotMapped] [JsonPropertyName("creator")]
    public string? Creator { get; set; }
    [NotMapped] [JsonPropertyName("creatorName")]
    public string? CreatorName { get; set; }
    [NotMapped] [JsonPropertyName("creatorLastName")]
    public string? CreatorLastName { get; set; }
    [Column("outcome")] [JsonPropertyName("outCome")]
    public string? OutCome { get; set; }
}
