// Converted from: camundaDomain/model/dto/ClaimTaskDto.java
// jakarta.validation @NotNull -> [Required] (System.ComponentModel.DataAnnotations)

using System.ComponentModel.DataAnnotations;
using SemBackDotNet10.camundaDomain.model.dto.@base;

namespace SemBackDotNet10.camundaDomain.model.dto;

public class ClaimTaskDto : ICamundaDto
{
    [Required]
    public string? UserId { get; set; }

    public ClaimTaskDto() { }

    public ClaimTaskDto(string userId)
    {
        UserId = userId;
    }
}
