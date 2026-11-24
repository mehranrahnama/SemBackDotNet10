// Converted from: camundaDomain/model/dto/TaskVariableItemDto.java
// lombok @Data/@AllArgsConstructor/@NoArgsConstructor -> property + سازنده‌ها

using SemBackDotNet10.camundaDomain.model.dto.@base;

namespace SemBackDotNet10.camundaDomain.model.dto;

public class TaskVariableItemDto : ICamundaDto
{
    public object? Value { get; set; }
    public string? Type { get; set; }
    public object? ValueInfo { get; set; }

    public TaskVariableItemDto() { }

    public TaskVariableItemDto(object? value, string? type, object? valueInfo)
    {
        Value = value;
        Type = type;
        ValueInfo = valueInfo;
    }
}
