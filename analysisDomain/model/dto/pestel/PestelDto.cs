using SemBackDotNet10.analysisDomain.model.entity.enums;

namespace SemBackDotNet10.analysisDomain.model.dto.pestel;

public class PestelDto
{
    public long? Id { get; set; }
    public long? PestelGroupId { get; set; }
    public string? PestelGroupTitle { get; set; }
    public string? Title { get; set; }
    public Impact? Impact { get; set; }
    public Uncertainty? Uncertainty { get; set; }

    public PestelDto()
    {
    }

    public PestelDto(long? id, long? pestelGroupId, string? title)
    {
        Id = id;
        PestelGroupId = pestelGroupId;
        Title = title;
    }

    public PestelDto(long? id, long? pestelGroupId, string? pestelGroupTitle, string? title)
    {
        Id = id;
        PestelGroupId = pestelGroupId;
        PestelGroupTitle = pestelGroupTitle;
        Title = title;
    }

    public PestelDto(long? id, long? pestelGroupId, string? pestelGroupTitle, string? title, Impact? impact, Uncertainty? uncertainty)
    {
        Id = id;
        PestelGroupId = pestelGroupId;
        PestelGroupTitle = pestelGroupTitle;
        Title = title;
        Impact = impact;
        Uncertainty = uncertainty;
    }
}
