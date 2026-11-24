using PageUtils = global::Common.utils.PageUtils;
using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using SemBackDotNet10.analysisDomain.repository.porter;
using SemBackDotNet10.analysisDomain.service.interf.porter;
using SemBackDotNet10.Common.exception;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.impl.porter;

public class ProductServiceImpl : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IProductGroupService _productGroupService;
    private readonly IProductThreatRepository _productThreatRepository;

    public ProductServiceImpl(IProductRepository productRepository,
        IProductGroupService productGroupService,
        IProductThreatRepository productThreatRepository)
    {
        _productRepository = productRepository;
        _productGroupService = productGroupService;
        _productThreatRepository = productThreatRepository;
    }

    public IProductRepository GetRepository() => _productRepository;

    public async Task<Product> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.Product.idIsNull");

        return await _productRepository.FindByIdAsync(id.Value, ct)
            ?? throw new BusinessException("error.Product.idIsNotValid");
    }

    public async Task<ProductDto> CreateNewProductAsync(ProductDto productDto, CancellationToken ct = default)
    {
        if (productDto == null)
            throw new BusinessException("error.productDto.isNull");

        var entity = await MapToEntityAsync(productDto, ct);
        if (entity.Id == 0)
            await _productRepository.AddAsync(entity, ct);
        else
            await _productRepository.UpdateAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task UpdateProductAsync(ProductDto productDto, CancellationToken ct = default)
    {
        var product = await FindByIdOrElseThrowAsync(productDto.Id, ct);

        if (productDto.ProductGroupDto != null)
        {
            var productGroup = await _productGroupService.FindByIdOrElseThrowAsync(productDto.ProductGroupDto.Id, ct);
            product.ProductGroup = productGroup;
            product.ProductGroupId = productGroup.Id;
        }

        if (productDto.Title != null)
            product.Title = productDto.Title;

        await _productRepository.UpdateAsync(product, ct);
    }

    public async Task DeleteProductByIdAsync(long productId, CancellationToken ct = default)
    {
        var productExists = await _productRepository.ExistsByIdAsync(productId, ct);
        var productThreatExists = await _productThreatRepository.ExistsByProductIdAsync(productId, ct);

        if (productExists && productThreatExists)
            throw new BusinessException("به علت وجود اطلاعات ارزیابی محرک برای محصول مورد نظر ،  امکان حذف آن وجود ندارد ");
        if (!productThreatExists && productExists)
        {
            var product = await _productRepository.FindByIdAsync(productId, ct);
            if (product != null)
                await _productRepository.DeleteAsync(product, ct);
        }
        else
            throw new BusinessException("خطا در حذف محصول");
    }

    public async Task<SearchDto> GetAllProductPageByProductGroupAsync(int page, int size, string? orderBy, SortDirection? direction, string? body, CancellationToken ct = default)
    {
        var json = PorterJsonHelper.Parse(body);
        var filter = PorterJsonHelper.GetString(json, "filter");
        var productGroupId = PorterJsonHelper.GetLong(json, "productGroupId");
        var pageRequest = PageUtils.GetInstance(page, size, orderBy, direction);
        var result = await _productRepository.FindAllPageAsync(productGroupId, filter, pageRequest, ct);
        return new SearchDto.Builder()
            .PageSize(size).CurrentPage(page).TotalPages(result.TotalPages)
            .TotalElements(result.TotalElements)
            .Content(result.Items)
            .Build();
    }

    public async Task<SearchDto> FindAllProductByPlanIdAsync(int page, int size, string? orderBy, SortDirection? direction, string? body, CancellationToken ct = default)
    {
        var pageRequest = PageUtils.GetInstance(page, size, orderBy, direction);
        var json = PorterJsonHelper.Parse(body);
        var filter = PorterJsonHelper.GetString(json, "filter");
        var planId = PorterJsonHelper.GetLong(json, "planId");
        var result = await _productRepository.FindAllByPlanIdAsync(planId, filter, pageRequest, ct);

        return new SearchDto.Builder()
            .PageSize(size).CurrentPage(page).TotalPages(result.TotalPages)
            .TotalElements(result.TotalElements)
            .Content(result.Items)
            .Build();
    }

    private ProductDto MapToDto(Product product) =>
        new()
        {
            Id = product.Id,
            Title = product.Title,
            ProductGroupDto = _productGroupService.MapToDto(product.ProductGroup)
        };

    private async Task<Product> MapToEntityAsync(ProductDto productDto, CancellationToken ct)
    {
        var productGroup = await _productGroupService.FindByIdOrElseThrowAsync(productDto.ProductGroupDto?.Id, ct);
        return new Product
        {
            Id = productDto.Id ?? 0,
            Title = productDto.Title ?? string.Empty,
            ProductGroup = productGroup,
            ProductGroupId = productGroup.Id
        };
    }
}
