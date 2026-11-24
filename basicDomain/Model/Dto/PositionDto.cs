#nullable enable

namespace SemBackDotNet10.basicDomain.Model.Dto;

public class PositionDto
{
    public string? DesPost { get; set; }
    public long? Id { get; set; }
    public string? CodPost { get; set; }

    public PositionDto() { }

    public PositionDto(long? id, string? desPost, string? codPost)
    {
        Id = id;
        DesPost = desPost;
        CodPost = codPost;
    }

    public PositionDto(string? desPost, string? codPost)
    {
        DesPost = desPost;
        CodPost = codPost;
    }
}
