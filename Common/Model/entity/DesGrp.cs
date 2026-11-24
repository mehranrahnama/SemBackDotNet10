// C# migration of model/entity/DesGrp.java

namespace Common.model.entity;

[Table("tbldesgrp", Schema = "dbo")]
public class DesGrp
{
    [Column("desgrpid")] [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [Column("desgrpe")] [MaxLength(50)] [JsonPropertyName("desGrpE")]
    public string? DesGrpE { get; set; }
    [Column("desgrpf")] [MaxLength(50)] [JsonPropertyName("desGrpF")]
    public string? DesGrpF { get; set; }
    [Column("comment")] [MaxLength(300)] [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}
