using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.utils.deserializer;
using SemBackDotNet10.actionDomain.model.dto;

namespace SemBackDotNet10.actionDomain.controller.dto.actionActPlanDtos;

// Source: Java UpdateActionActPlanDto record. Nullable types preserve missing input
// until validation; Month is positive, not constrained to 12 in this contract.
public class UpdateActionActPlanDto
{
    [JsonPropertyName("planValue")]
    [JsonConverter(typeof(OptionalBigDecimalDeserializer))]
    [PlanValueDigits]
    public decimal? PlanValue { get; set; }

    [JsonPropertyName("actionActPlanId")]
    [JsonConverter(typeof(OptionalLongDeserializer))]
    [Required(ErrorMessage = "شناسه برنامه ریزی الزامی است.")]
    [Range(typeof(long), "1", "9223372036854775807", ErrorMessage = "شناسه برنامه ریزی باید عدد مثبت باشد.")]
    public long? ActionActPlanId { get; set; }

    [JsonPropertyName("planId")]
    [JsonConverter(typeof(OptionalLongDeserializer))]
    [Required(ErrorMessage = "سال برنامه ریزی الزامی است.")]
    [Range(typeof(long), "1", "9223372036854775807", ErrorMessage = "سال برنامه ریزی باید عدد مثبت باشد.")]
    public long? PlanId { get; set; }

    [JsonPropertyName("month")]
    [JsonConverter(typeof(OptionalLongDeserializer))]
    [Required(ErrorMessage = "ماه برنامه ریزی الزامی است.")]
    [Range(typeof(long), "1", "9223372036854775807", ErrorMessage = "ماه برنامه ریزی باید عدد مثبت باشد.")]
    public long? Month { get; set; }
}
