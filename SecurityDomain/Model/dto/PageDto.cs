using System.Text.Json.Serialization;

namespace SemBackDotNet10.SecurityDomain.Model.Dto;

public class PageDto
{
    public long? Id { get; set; }
    public int ParentId { get; set; }
    public string? DesF { get; set; }
    public List<PageDto>? Children { get; set; }

    // Jackson serialises the Java `isChecked()` getter as "checked".
    [JsonPropertyName("checked")]
    public bool IsChecked { get; set; }

    public PageDto()
    {
    }

    public PageDto(long? id, int parentId, string? desF, List<PageDto>? children, bool isChecked)
    {
        Id = id;
        ParentId = parentId;
        DesF = desF;
        Children = children;
        IsChecked = isChecked;
    }

    public PageDto(long? id, int parentId, string? desF, bool isChecked)
    {
        Id = id;
        ParentId = parentId;
        DesF = desF;
        IsChecked = isChecked;
    }
}
