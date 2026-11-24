// C# migration of model/entity/ReportExecutedData.java

namespace Common.model.entity;

[Table("tblreportexecuteddata", Schema = "dbo")]
public class ReportExecutedData
{
    [Column("reportexecuteddataid")] [Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [Column("storedprocedurename")] [JsonPropertyName("storedProcedureName")]
    public string? StoredProcedureName { get; set; }
    [Column("planid")] [JsonPropertyName("planId")]
    public long? PlanId { get; set; }
    [Column("month")] [JsonPropertyName("month")]
    public long? Month { get; set; }
    [Column("levelid")] [JsonPropertyName("levelId")]
    public long? LevelId { get; set; }
    [Column("lasttimeupdated")] [JsonPropertyName("lastTimeUpdated")]
    public DateTime? LastTimeUpdated { get; set; }
}
