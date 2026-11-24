// C# migration of model/entity/Des.java

namespace Common.model.entity;

[Table("tbldes", Schema = "dbo")]
public class Des
{
    [Column("desid")] [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [Column("dese")] [MaxLength(50)] [JsonPropertyName("desE")]
    public string? DesE { get; set; }
    [Column("desf")] [MaxLength(50)] [JsonPropertyName("desF")]
    public string? DesF { get; set; }
    [Column("val")] [MaxLength(50)] [JsonPropertyName("val")]
    public string? Val { get; set; }
    [JsonPropertyName("desGrp")]
    public global::Common.model.entity.DesGrp? DesGrp { get; set; }
}
