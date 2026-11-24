using System.Text.Json.Serialization;

namespace SemBackDotNet10.dashboardDomain.model.dto;

public class DashboardComprehensiveDto
{
    public int Id { get; set; }
    public long? PlanId { get; set; }
    public long? LevelId { get; set; }
    public string? DesLevel { get; set; }

    [JsonPropertyName("pcneachLevel")]
    public decimal? PCNEachLevel { get; set; }

    [JsonPropertyName("actionPayeshdCount_ByLevelIdAndMonth")]
    public int ActionPayeshdCount_ByLevelIdAndMonth { get; set; }
    [JsonPropertyName("actionNotPayeshdCount_ByLevelIdAndMonth")]
    public int ActionNotPayeshdCount_ByLevelIdAndMonth { get; set; }
    [JsonPropertyName("measurePayeshedCount_ByLevelIdAndMonth")]
    public int MeasurePayeshedCount_ByLevelIdAndMonth { get; set; }
    [JsonPropertyName("measureNotPayeshedCount_ByLevelIdAndMonth")]
    public int MeasureNotPayeshedCount_ByLevelIdAndMonth { get; set; }
    [JsonPropertyName("actionPCN_Achieved_Count_ByLevelIdAndMonth")]
    public int ActionPCN_Achieved_Count_ByLevelIdAndMonth { get; set; }
    [JsonPropertyName("actionPCN_Not_Achieved_Count_ByLevelIdAndMonth")]
    public int ActionPCN_Not_Achieved_Count_ByLevelIdAndMonth { get; set; }
    [JsonPropertyName("measurePCN_Achieved_Count_ByLevelIdAndMonth")]
    public int MeasurePCN_Achieved_Count_ByLevelIdAndMonth { get; set; }
    [JsonPropertyName("measurePCN_Not_Achieved_Count_ByLevelIdAndMonth")]
    public int MeasurePCN_Not_Achieved_Count_ByLevelIdAndMonth { get; set; }
    [JsonPropertyName("objectPCN_Achieved_Count_ByLevelIdAndMonth")]
    public int ObjectPCN_Achieved_Count_ByLevelIdAndMonth { get; set; }
    [JsonPropertyName("objectPCN_Not_Achieved_Count_ByLevelIdAndMonth")]
    public int ObjectPCN_Not_Achieved_Count_ByLevelIdAndMonth { get; set; }
}
