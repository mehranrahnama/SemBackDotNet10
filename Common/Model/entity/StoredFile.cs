// C# migration of model/entity/StoredFile.java

namespace Common.model.entity;

[Table("stored_file", Schema = "attach")]
public class StoredFile
{
    [Column("file_id")] [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [JsonPropertyName("fileId")]
    public long FileId { get; set; }
    [JsonPropertyName("data")]
    public byte[]? Data { get; set; }
}
