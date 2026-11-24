namespace SemBackDotNet10.analysisDomain.model.dto.porter;

public class AltProductDto
{
    public long? Id { get; set; }
    public ProductDto? ProductDto { get; set; }
    public string? Title { get; set; }

    public AltProductDto()
    {
    }

    public AltProductDto(long? id, ProductDto? productDto, string? title)
    {
        Id = id;
        ProductDto = productDto;
        Title = title;
    }

    public AltProductDto(long? id, long? productId, string? productTitle, string? title)
    {
        Id = id;
        ProductDto = new ProductDto(productId, productTitle);
        Title = title;
    }
}
