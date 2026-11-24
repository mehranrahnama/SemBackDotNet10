using Common.exception;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.enums;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using SemBackDotNet10.analysisDomain.repository.porter;
using SemBackDotNet10.analysisDomain.service.interf.porter;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.analysisDomain.service.impl.porter;

public class PorterResultServiceImpl : IPorterResultService
{
    private readonly IPorterResultRepository _porterResultRepository;
    private readonly IProductService _productService;
    private readonly IMarketService _marketService;
    private readonly IPorterGroupService _porterGroupService;
    private readonly global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository _planPeriodService;

    public PorterResultServiceImpl(IPorterResultRepository porterResultRepository,
        IProductService productService,
        IMarketService marketService,
        IPorterGroupService porterGroupService,
        global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository planPeriodService)
    {
        _porterResultRepository = porterResultRepository;
        _productService = productService;
        _marketService = marketService;
        _porterGroupService = porterGroupService;
        _planPeriodService = planPeriodService;
    }

    public IPorterResultRepository GetRepository() => _porterResultRepository;

    public async Task<PorterResult> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.PorterResult.idIsNull");

        return await _porterResultRepository.FindByIdAsync(id.Value, ct)
            ?? throw new BusinessException("error.PorterResult.idIsNotValid");
    }

    public async Task CreateNewPorterResultAsync(PorterResultDto porterResultDto, CancellationToken ct = default)
    {
        var entity = await MapToEntityAsync(porterResultDto, ct);
        if (entity.Id == 0)
            await _porterResultRepository.AddAsync(entity, ct);
        else
            await _porterResultRepository.UpdateAsync(entity, ct);
    }

    public Task<PorterResultDto?> GetAllByProductMarketAsync(string? body, CancellationToken ct = default)
    {
        var json = PorterJsonHelper.Parse(body);
        var productId = PorterJsonHelper.OptLong(json, "productId");
        var marketId = PorterJsonHelper.OptLong(json, "marketId");
        var porterGroupId = PorterJsonHelper.OptLong(json, "porterGroupId");
        return _porterResultRepository.GetAllByProductMarketAsync(productId, marketId, porterGroupId, ct);
    }

    public async Task<PorterResultDto> EditPorterResultAsync(PorterResultDto porterResultDto, CancellationToken ct = default)
    {
        var porterResult = await FindByIdOrElseThrowAsync(porterResultDto.Id, ct);
        porterResult.Result = ThreatExtensions.FromId(porterResultDto.ThreatDto?.Id);
        await _porterResultRepository.UpdateAsync(porterResult, ct);

        var detailed = await _porterResultRepository.Query
            .Include(pr => pr.Product).ThenInclude(p => p.ProductGroup)
            .Include(pr => pr.Market)
            .Include(pr => pr.PorterGroup)
            .FirstAsync(pr => pr.Id == porterResult.Id, ct);
        return MapToDto(detailed);
    }

    public async Task<PorterResult> MapToEntityAsync(PorterResultDto porterResultDto, CancellationToken ct = default)
    {
        var product = await _productService.FindByIdOrElseThrowAsync(porterResultDto.ProductDto?.Id, ct);
        var market = await _marketService.FindByIdOrElseThrowAsync(porterResultDto.MarketDto?.Id, ct);
        var porterGroup = await _porterGroupService.FindByIdOrElseThrowAsync(porterResultDto.PorterGroupDto?.Id, ct);
        var planPeriod = await _planPeriodService.FindPlanPeriodByPlanIdAsync(
            porterResultDto.PlanId ?? throw new BusinessException("error.planPeriod.notFound"), ct);

        return new PorterResult
        {
            Id = porterResultDto.Id ?? 0,
            Product = product,
            ProductId = product.Id,
            Market = market,
            MarketId = market.Id,
            PorterGroup = porterGroup,
            PorterGroupId = porterGroup.Id,
            PlanPeriod = planPeriod,
            PlanId = planPeriod.PlanId,
            Result = ThreatExtensions.FromId(porterResultDto.ThreatDto?.Id),
            Description = porterResultDto.Description ?? string.Empty
        };
    }

    public PorterResultDto MapToDto(PorterResult porterResult) =>
        new()
        {
            Id = porterResult.Id,
            ProductDto = new ProductDto(porterResult.Product.Id, porterResult.Product.Title,
                porterResult.Product.ProductGroup.Id, porterResult.Product.ProductGroup.Title),
            MarketDto = new MarketDto(porterResult.Market.Id, porterResult.Market.Title, porterResult.Market.PlanId),
            PorterGroupDto = new PorterGroupDto(porterResult.PorterGroup.Id, porterResult.PorterGroup.Title,
                porterResult.PorterGroup.PlanId),
            Description = porterResult.Description
        };
}
