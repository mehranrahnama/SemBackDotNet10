// C# migration of model/entity/Attachment.java

namespace Common.model.entity;

[Table("attachment", Schema = "dbo")]
public class Attachment
{
    [Column("attachmentid")] [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [Column("filename")] [MaxLength(200)] [JsonPropertyName("fileName")]
    public string? FileName { get; set; }
    [Column("entityname")] [MaxLength(50)] [JsonPropertyName("entityName")]
    public string? EntityName { get; set; }
    [Column("entityid")] [JsonPropertyName("entityId")]
    public long? EntityId { get; set; }
    [Column("Confirmstatus")] [JsonPropertyName("ConfirmStatus")]
    public long? ConfirmStatus { get; set; }
    [Column("uploadurl")] [MaxLength(100)] [JsonPropertyName("uploadUrl")]
    public string? UploadUrl { get; set; }
    [Column("des")] [MaxLength(200)] [JsonPropertyName("des")]
    public string? Des { get; set; }
    [Column("deleted")] [JsonPropertyName("deleted")]
    public bool? Deleted { get; set; } = false;
}
