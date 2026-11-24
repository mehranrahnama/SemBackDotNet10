using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace SemBackDotNet10.basicDomain.Model.Dto;

/// <summary>
/// C# record equivalent of the Java record:
///   record LoadStrategyObjectListDto(@NotNull Long levelId, @NotNull Long planId, @Size(max = 100) String filter)
/// </summary>
[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public record LoadStrategyObjectListDto(
    [Required(ErrorMessage = "شناسه سطح برنامه ریزی اجباریست.")]
    long LevelId,

    [Required(ErrorMessage = "شناسه سال برنامه ریزی اجباریست.")]
    long PlanId,

    [StringLength(100, ErrorMessage = "فیلتر میتواند ماکزیمم 100 کاراکتر داشته باشد.")]
    string? Filter);
