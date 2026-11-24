// C# migration of model/entity/StoredAttachment.java

namespace Common.model.entity;

[Table("stored_attachment", Schema = "attach")]
public class StoredAttachment
{
    [Column("attachment_id")] [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [Column("resource")] [JsonPropertyName("resource")]
    public global::Common.enums.EntityNames? Resource { get; set; }
    [Column("entity_id")] [JsonPropertyName("entityId")]
    public string? EntityId { get; set; }
    [Column("row_no")] [JsonPropertyName("rowNo")]
    public int? RowNo { get; set; }
    [Column("description")] [MaxLength(500)] [JsonPropertyName("description")]
    public string? Description { get; set; }
    [Column("file_name")] [MaxLength(100)] [JsonPropertyName("fileName")]
    public string? FileName { get; set; }
    [Column("file_type")] [JsonPropertyName("fileType")]
    public string? FileType { get; set; }
    [Column("file_size")] [JsonPropertyName("fileSize")]
    public long? FileSize { get; set; }
    [JsonPropertyName("storedFile")]
    public global::Common.model.entity.StoredFile? StoredFile { get; set; }
}
