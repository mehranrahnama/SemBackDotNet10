#nullable enable
using System.Text.Json.Serialization;

namespace SemBackDotNet10.actionDomain.model.dto;

// Java record ActionActPlanReplanDto: nullable Long/BigDecimal components.
public sealed record ActionActPlanReplanDto(
    [property: JsonPropertyName("actionActPlanId")] long? ActionActPlanId,
    [property: JsonPropertyName("planId")] long? PlanId,
    [property: JsonPropertyName("month")] long? Month,
    [property: JsonPropertyName("prevPlanValue")] decimal? PrevPlanValue,
    [property: JsonPropertyName("newPlanValue")] decimal? NewPlanValue,
    [property: JsonPropertyName("prevActualValue")] decimal? PrevActualValue,
    [property: JsonPropertyName("newActualValue")] decimal? NewActualValue);
