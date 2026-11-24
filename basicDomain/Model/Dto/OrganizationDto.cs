namespace SemBackDotNet10.basicDomain.Model.Dto;

/// <summary>معادل OrganizationDto جاوا.</summary>
public class OrganizationDto
{
    public long Id { get; set; }

    public long? OrganId { get; set; }

    public long? RowNumber { get; set; }

    public string? DesLevel { get; set; }

    public string? DesOrganF { get; set; }

    public long? ModirId { get; set; }

    public string? ModirFullName { get; set; }
}
