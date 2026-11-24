// Mechanical conversion from Java (controller/dto/UpdateActivityProgressRequestDto.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
namespace SemBackDotNet10.actionDomain.controller.dto;

/// <summary>معادل UpdateActivityProgressRequestDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class UpdateActivityProgressRequestDto
{
    public long? ActivityProgressId { get; set; }
    public decimal? ActualValue { get; set; }
}
