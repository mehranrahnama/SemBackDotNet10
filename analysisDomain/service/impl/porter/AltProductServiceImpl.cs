using Common.exception;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using SemBackDotNet10.analysisDomain.repository.porter;
using SemBackDotNet10.analysisDomain.service.interf.porter;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.analysisDomain.service.impl.porter;

public class AltProductServiceImpl : IAltProductService
{
    private readonly IAltProductRepository _altProductRepository;
    private readonly IProductService _productService;

    public AltProductServiceImpl(IAltProductRepository altProductRepository, IProductService productService)
    {
        _altProductRepository = altProductRepository;
        _productService = productService;
    }

    public IAltProductRepository GetRepository() => _altProductRepository;

    public async Task<AltProduct> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.AltProduct.idIsNull");
        return await _altProductRepository.FindByIdAsync(id.Value, ct)
            ?? throw new BusinessException("error.AltProduct.idIsNotValid");
    }

    public async Task<AltProductDto> CreateNewAltProductAsync(AltProductDto altProductDto, CancellationToken ct = default)
    {
        if (altProductDto == null)
            throw new BusinessException("error.altProductDto.isNull");

        var entity = await MapToEntityAsync(altProductDto, ct);
        var result = entity.Id == 0
            ? await _altProductRepository.AddAsync(entity, ct)
            : await UpdateAndReturnAsync(entity, ct);
        return MapToDto(result);
    }

    public async Task UpdateAltProductAsync(AltProductDto altProductDto, CancellationToken ct = default)
    {
        var altProduct = await FindByIdOrElseThrowAsync(altProductDto.Id, ct);

        if (altProductDto.Title != null)
            altProduct.Title = altProductDto.Title;

        await _altProductRepository.UpdateAsync(altProduct, ct);
    }

    public async Task DeleteAltProductByIdAsync(long? altProductId, CancellationToken ct = default)
    {
        var result = await FindByIdOrElseThrowAsync(altProductId, ct);
        await _altProductRepository.DeleteAsync(result, ct);
    }

    public Task<List<AltProductDto>> GetAllAltProductByProductIdAsync(long? productId, CancellationToken ct = default) =>
        _altProductRepository.GetAllAltProductByProductIdAsync(productId, ct);

    private async Task<AltProduct> UpdateAndReturnAsync(AltProduct entity, CancellationToken ct)
    {
        await _altProductRepository.UpdateAsync(entity, ct);
        return entity;
    }

    private static AltProductDto MapToDto(AltProduct altProduct) =>
        new()
        {
            Id = altProduct.Id,
            Title = altProduct.Title,
            ProductDto = new ProductDto(altProduct.Product.Id, altProduct.Product.Title)
        };

    private async Task<AltProduct> MapToEntityAsync(AltProductDto altProductDto, CancellationToken ct)
    {
        var product = await _productService.FindByIdOrElseThrowAsync(altProductDto.ProductDto?.Id, ct);
        return new AltProduct
        {
            Id = altProductDto.Id ?? 0,
            Title = altProductDto.Title,
            Product = product,
            ProductId = product.Id
        };
    }
}
