// Converted from: camundaDomain/model/dto/CamundaFormRefDto.java

namespace SemBackDotNet10.camundaDomain.model.dto;

public class CamundaFormRefDto
{
    public string? Key { get; set; }
    public string? Binding { get; set; }
    public int Version { get; set; }

    public CamundaFormRefDto() { }

    public CamundaFormRefDto(string? key, string? binding, int version)
    {
        Key = key;
        Binding = binding;
        Version = version;
    }
}
