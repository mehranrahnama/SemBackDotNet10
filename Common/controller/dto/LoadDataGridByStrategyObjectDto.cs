// C# migration of controller/dto/LoadDataGridByStrategyObjectDto.java

namespace Common.controller.dto;

public class LoadDataGridByStrategyObjectDto
{
    [JsonPropertyName("filter")]
    public string? Filter { get; set; }
    [JsonPropertyName("levelId")]
    public long? LevelId { get; set; }
    [JsonPropertyName("status")]
    public long? Status { get; set; }
    [JsonPropertyName("objectId")]
    public long? ObjectId { get; set; }
    [JsonPropertyName("planId")]
    public long? PlanId { get; set; }
}
