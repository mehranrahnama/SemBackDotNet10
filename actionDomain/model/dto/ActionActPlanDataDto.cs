using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;

namespace SemBackDotNet10.actionDomain.model.dto;

// Value equality mirrors Lombok @Data, including BigDecimal's scale-sensitive equality.
// Like the mutable Java DTO, do not mutate a value while it is held in a HashSet.
public sealed record ActionActPlanDataDto
{
    [JsonPropertyName("actionPlanCatalogId")]
    [Required(ErrorMessage = "شناسه کاتالوگ اقدام الزامی است.")]
    [Range(typeof(long), "1", "9223372036854775807", ErrorMessage = "شناسه کاتالوگ اقدام باید عدد مثبت باشد.")]
    public long? ActionPlanCatalogId { get; set; }

    [JsonPropertyName("planId")]
    [Required(ErrorMessage = "شناسه دوره برنامه الزامی است.")]
    [Range(typeof(long), "1", "9223372036854775807", ErrorMessage = "شناسه دوره برنامه باید عدد مثبت باشد.")]
    public long? PlanId { get; set; }

    [JsonPropertyName("month")]
    [Required(ErrorMessage = "ماه الزامی است.")]
    [Range(1L, 12L, ErrorMessage = "ماه باید بین ۱ تا ۱۲ باشد.")]
    public long? Month { get; set; }

    [JsonPropertyName("planValue")]
    [PlanValueDigits]
    public decimal? PlanValue { get; set; }

    [JsonPropertyName("actionActPlanId")]
    [Required(ErrorMessage = "شناسه برنامه ریزی الزامی است.")]
    [Range(typeof(long), "1", "9223372036854775807", ErrorMessage = "شناسه برنامه ریزی باید عدد مثبت باشد.")]
    public long? ActionActPlanId { get; set; }

    [JsonPropertyName("planned")]
    public bool IsPlanned { get; set; }

    public ActionActPlanDataDto() { }

    public ActionActPlanDataDto(long? actionPlanCatalogId, long? planId, long? month,
        decimal? planValue, long? actionActPlanId, bool isPlanned)
    {
        ActionPlanCatalogId = actionPlanCatalogId;
        PlanId = planId;
        Month = month;
        PlanValue = planValue;
        ActionActPlanId = actionActPlanId;
        IsPlanned = isPlanned;
    }

    private static int Scale(decimal? value)
        => value.HasValue ? (decimal.GetBits(value.Value)[3] >> 16) & 0xff : -1;

    public bool Equals(ActionActPlanDataDto? other)
        => other is not null
        && ActionPlanCatalogId == other.ActionPlanCatalogId
        && PlanId == other.PlanId && Month == other.Month
        && PlanValue == other.PlanValue && Scale(PlanValue) == Scale(other.PlanValue)
        && ActionActPlanId == other.ActionActPlanId && IsPlanned == other.IsPlanned;

    public override int GetHashCode()
        => HashCode.Combine(ActionPlanCatalogId, PlanId, Month, PlanValue,
            Scale(PlanValue), ActionActPlanId, IsPlanned);
}

// Java @Digits(integer=20, fraction=4); null is allowed in the source contract.
[AttributeUsage(AttributeTargets.Property)]
internal sealed class PlanValueDigitsAttribute : ValidationAttribute
{
    public PlanValueDigitsAttribute()
        : base("مقدار برنامه ریزی باید حداکثر ۲۰ رقم صحیح و ۴ رقم اعشار داشته باشد.") { }

    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        if (value is not decimal number) return false;
        var scale = (decimal.GetBits(number)[3] >> 16) & 0xff;
        var integerDigits = decimal.Truncate(number).ToString(CultureInfo.InvariantCulture)
            .TrimStart('-').Length;
        return scale <= 4 && integerDigits <= 20;
    }
}
