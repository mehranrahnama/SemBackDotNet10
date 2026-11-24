namespace SemBackDotNet10.SecurityDomain.Model.Dto;

public class PageAccessDto
{
    public long? Id { get; set; }
    public string? DesF { get; set; }
    public string? Uri { get; set; }
    public string? PageName { get; set; }

    public PageAccessDto()
    {
    }

    public PageAccessDto(long? id, string? desF, string? uri, string? pageName)
    {
        Id = id;
        DesF = desF;
        Uri = uri;
        PageName = pageName;
    }
}
