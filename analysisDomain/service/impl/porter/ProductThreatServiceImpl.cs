using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.enums;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using SemBackDotNet10.analysisDomain.repository.porter;
using SemBackDotNet10.analysisDomain.service.interf.porter;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.analysisDomain.service.impl.porter;

public class ProductThreatServiceImpl : IProductThreatService
{
    private readonly IProductThreatRepository _productThreatRepository;
    private readonly IProductService _productService;
    private readonly IMarketService _marketService;
    private readonly IPorterService _porterService;

    public ProductThreatServiceImpl(IProductThreatRepository productThreatRepository,
        IProductService productService,
        IMarketService marketService,
        IPorterService porterService)
    {
        _productThreatRepository = productThreatRepository;
        _productService = productService;
        _marketService = marketService;
        _porterService = porterService;
    }

    public IProductThreatRepository GetRepository() => _productThreatRepository;

    public async Task<ProductThreat> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.ProductThreat.idIsNull");

        return await _productThreatRepository.FindByIdAsync(id.Value, ct)
            ?? throw new BusinessException("error.ProductThreat.idIsNotValid");
    }

    public async Task CreateNewProductThreatAsync(List<ProductThreatDto> productThreatDto, CancellationToken ct = default)
    {
        var transientEntities = new List<ProductThreat>();
        foreach (var dto in productThreatDto)
            transientEntities.Add(await MapToEntityAsync(dto, ct));

        var toAdd = transientEntities.Where(e => e.Id == 0).ToList();
        var toUpdate = transientEntities.Where(e => e.Id != 0).ToList();
        if (toAdd.Count > 0)
            await _productThreatRepository.AddRangeAsync(toAdd, ct);
        if (toUpdate.Count > 0)
            await _productThreatRepository.UpdateRangeAsync(toUpdate, ct);
    }

    public Task<List<ProductThreatDto>> GetAllByProductMarketAsync(string? body, CancellationToken ct = default)
    {
        var json = PorterJsonHelper.Parse(body);
        var productId = PorterJsonHelper.OptLong(json, "productId");
        var marketId = PorterJsonHelper.OptLong(json, "marketId");
        var porterGroupId = PorterJsonHelper.OptLong(json, "porterGroupId");
        return _productThreatRepository.GetAllByProductMarketAsync(productId, marketId, porterGroupId, ct);
    }

    public async Task<List<ProductThreatDto>> EditProductThreatAsync(List<ProductThreatDto> productThreatDto, CancellationToken ct = default)
    {
        foreach (var dto in productThreatDto)
        {
            var productThreat = await FindByIdOrElseThrowAsync(dto.Id, ct);
            productThreat.Threat = ThreatExtensions.FromId(dto.ThreatDto?.Id);
            await _productThreatRepository.UpdateAsync(productThreat, ct);
        }

        var first = productThreatDto[0];
        return await _productThreatRepository.GetAllByProductMarketAsync(
            first.ProductDto?.Id,
            first.MarketDto?.Id,
            first.PorterDto?.PorterGroupDto?.Id,
            ct);
    }

    public async Task<ProductThreat> MapToEntityAsync(ProductThreatDto productThreatDto, CancellationToken ct = default)
    {
        var product = await _productService.FindByIdOrElseThrowAsync(productThreatDto.ProductDto?.Id, ct);
        var market = await _marketService.FindByIdOrElseThrowAsync(productThreatDto.MarketDto?.Id, ct);
        var porter = await _porterService.FindByIdOrElseThrowAsync(productThreatDto.PorterDto?.Id, ct);
        return new ProductThreat
        {
            Id = productThreatDto.Id ?? 0,
            Product = product,
            ProductId = product.Id,
            Market = market,
            MarketId = market.Id,
            Porter = porter,
            PorterId = porter.Id,
            Threat = ThreatExtensions.FromId(productThreatDto.ThreatDto?.Id)
        };
    }

    public ProductThreatDto MapToDto(ProductThreat productThreat) =>
        new()
        {
            Id = productThreat.Id,
            ProductDto = new ProductDto(productThreat.Product.Id, productThreat.Product.Title,
                productThreat.Product.ProductGroup.Id, productThreat.Product.ProductGroup.Title),
            MarketDto = new MarketDto(productThreat.Market.Id, productThreat.Market.Title, productThreat.Market.PlanId),
            PorterDto = new PorterDto(productThreat.Porter.Id, productThreat.Porter.Title,
                productThreat.Porter.NoThreat, productThreat.Porter.MiddleThreat, productThreat.Porter.SeriousThreat,
                productThreat.Porter.PorterGroup.Id, productThreat.Porter.PorterGroup.Title)
        };
}
