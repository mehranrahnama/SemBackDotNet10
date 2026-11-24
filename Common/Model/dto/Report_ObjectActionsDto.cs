// C# migration of model/dto/Report_ObjectActionsDto.java

namespace Common.model.dto;

public class Report_ObjectActionsDto
{
    [JsonPropertyName("desPerspectF")]
    public string? DesPerspectF { get; set; }
    [JsonPropertyName("hadafName")]
    public string? HadafName { get; set; }
    [JsonPropertyName("actionName")]
    public string? ActionName { get; set; }
    [JsonPropertyName("startDate")]
    public string? StartDate { get; set; }
    [JsonPropertyName("endDate")]
    public string? EndDate { get; set; }
    [JsonPropertyName("actionWeight")]
    public float? ActionWeight { get; set; }
    [JsonPropertyName("actionType")]
    public string? ActionType { get; set; }
    [JsonPropertyName("modir")]
    public string? Modir { get; set; }
    [JsonPropertyName("measureName")]
    public string? MeasureName { get; set; }
    [JsonPropertyName("measureWeight")]
    public float? MeasureWeight { get; set; }
    [JsonPropertyName("measureUnit")]
    public string? MeasureUnit { get; set; }
    [JsonPropertyName("measureLastYearValue")]
    public float? MeasureLastYearValue { get; set; }
    [JsonPropertyName("measureCurrentYearTarget")]
    public float? MeasureCurrentYearTarget { get; set; }
}
