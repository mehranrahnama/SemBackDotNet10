namespace SemBackDotNet10.analysisDomain.model.dto.porter;

public class ProductDto
{
    public long? Id { get; set; }
    public ProductGroupDto? ProductGroupDto { get; set; }
    public string? Title { get; set; }

    public ProductDto()
    {
    }

    public ProductDto(long? id, ProductGroupDto? productGroupDto, string? title)
    {
        Id = id;
        ProductGroupDto = productGroupDto;
        Title = title;
    }

    public ProductDto(long? id, string? title, long? productGroupId, string? productGroupTitle)
    {
        Id = id;
        Title = title;
        ProductGroupDto = new ProductGroupDto(productGroupId, productGroupTitle);
    }

    public ProductDto(long? id, string? title)
    {
        Id = id;
        Title = title;
    }
}
