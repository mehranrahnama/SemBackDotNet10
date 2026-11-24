#nullable enable

namespace SemBackDotNet10.basicDomain.Model.Dto;

public class MeasureUnitExcelDto
{
    public long? Id { get; set; }
    public string? DesMeasureE { get; set; }
    public string? DesMeasureF { get; set; }
    public bool? IsCumulative { get; set; }
    public string? Code { get; set; }

    public MeasureUnitExcelDto() { }

    public MeasureUnitExcelDto(long? id, string? desMeasureE, string? desMeasureF, bool? isCumulative, string? code)
    {
        Id = id;
        DesMeasureE = desMeasureE;
        DesMeasureF = desMeasureF;
        IsCumulative = isCumulative;
        Code = code;
    }
}
