namespace SemBackDotNet10.analysisDomain.model.dto.pestel;

public class PestelEnumsValueDto
{
    public int OrdinalValue { get; set; }
    public string? StringValue { get; set; }
    public string? EnumsType { get; set; }

    public PestelEnumsValueDto()
    {
    }

    public PestelEnumsValueDto(int ordinalValue, string? stringValue, string? enumsType)
    {
        OrdinalValue = ordinalValue;
        StringValue = stringValue;
        EnumsType = enumsType;
    }
}
