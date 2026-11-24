using PageUtils = global::Common.utils.PageUtils;
using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using SemBackDotNet10.analysisDomain.repository.porter;
using SemBackDotNet10.analysisDomain.service.interf.porter;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.Common.exception;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.impl.porter;

public class PorterScoreServiceImpl : IPorterScoreService
{
    private readonly IPorterScoreRepository _porterScoreRepository;
    private readonly IPorterGroupService _porterGroupService;
    private readonly global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository _planPeriodService;
    private readonly IMarketService _marketService;
    private readonly IProductService _productService;

    public PorterScoreServiceImpl(IPorterScoreRepository porterScoreRepository,
        IPorterGroupService porterGroupService,
        global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository planPeriodService,
        IMarketService marketService,
        IProductService productService)
    {
        _porterScoreRepository = porterScoreRepository;
        _porterGroupService = porterGroupService;
        _planPeriodService = planPeriodService;
        _marketService = marketService;
        _productService = productService;
    }

    public IPorterScoreRepository GetRepository() => _porterScoreRepository;

    public async Task<PorterScore> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.PorterScore.idIsNotValid");

        return await _porterScoreRepository.FindByIdAsync(id.Value, ct)
            ?? throw new BusinessException("error.PorterScore.idIsNotValid");
    }

    public async Task CreateNewPorterScoreAsync(List<PorterScoreDto> porterScoreDtos, CancellationToken ct = default)
    {
        var transientEntities = new List<PorterScore>();
        foreach (var porterScoreDto in porterScoreDtos)
            transientEntities.Add(await MapToEntityAsync(porterScoreDto, ct));

        var toAdd = transientEntities.Where(e => e.Id == 0).ToList();
        var toUpdate = transientEntities.Where(e => e.Id != 0).ToList();
        if (toAdd.Count > 0)
            await _porterScoreRepository.AddRangeAsync(toAdd, ct);
        if (toUpdate.Count > 0)
            await _porterScoreRepository.UpdateRangeAsync(toUpdate, ct);
    }

    public async Task DeletePorterScoreAsync(long? porterScoreId, CancellationToken ct = default)
    {
        var porterScore = await FindByIdOrElseThrowAsync(porterScoreId, ct);
        await _porterScoreRepository.DeleteAsync(porterScore, ct);
    }

    public async Task EditPorterScoreAsync(List<PorterScoreDto> porterScoreDtos, CancellationToken ct = default)
    {
        foreach (var porterScoreDto in porterScoreDtos)
        {
            if (porterScoreDto.Id != null)
            {
                var porterScore = await FindByIdOrElseThrowAsync(porterScoreDto.Id, ct);
                porterScore.Title = porterScoreDto.Title ?? string.Empty;
                if (porterScoreDto.PorterGroupDto?.Id != null)
                {
                    var porterGroup = await _porterGroupService.FindByIdOrElseThrowAsync(porterScoreDto.PorterGroupDto.Id, ct);
                    porterScore.PorterGroup = porterGroup;
                    porterScore.PorterGroupId = porterGroup.Id;
                }

                if (porterScoreDto.PlanId != null)
                {
                    var planPeriod = await _planPeriodService.FindPlanPeriodByPlanIdAsync(porterScoreDto.PlanId.Value, ct);
                    porterScore.PlanPeriod = planPeriod;
                    porterScore.PlanId = planPeriod.PlanId;
                }

                if (porterScoreDto.MarketDto != null)
                {
                    var market = await _marketService.FindByIdOrElseThrowAsync(porterScoreDto.MarketDto.Id, ct);
                    porterScore.Market = market;
                    porterScore.MarketId = market.Id;
                }

                if (porterScoreDto.ProductDto != null)
                {
                    var product = await _productService.FindByIdOrElseThrowAsync(porterScoreDto.ProductDto.Id, ct);
                    porterScore.Product = product;
                    porterScore.ProductId = product.Id;
                }

                porterScore.CurrentScore = porterScoreDto.CurrentScore;
                porterScore.FutureScore = porterScoreDto.FutureScore;
                await _porterScoreRepository.UpdateAsync(porterScore, ct);
            }
            else
            {
                await CreateNewPorterScoreAsync(new List<PorterScoreDto> { porterScoreDto }, ct);
            }
        }
    }

    public async Task EditScoreAsync(List<PorterScoreDto> porterScoreDtos, CancellationToken ct = default)
    {
        foreach (var porterScoreDto in porterScoreDtos)
        {
            var porterScore = await FindByIdOrElseThrowAsync(porterScoreDto.Id, ct);
            porterScore.CurrentScore = porterScoreDto.CurrentScore;
            porterScore.FutureScore = porterScoreDto.FutureScore;
            await _porterScoreRepository.UpdateAsync(porterScore, ct);
        }
    }

    public async Task<SearchDto> GetAllByPorterGroupIdAsync(long? porterGroupId, int page, int size, string? orderBy, SortDirection? direction, CancellationToken ct = default)
    {
        var pageRequest = PageUtils.GetInstance(page, size, orderBy, direction);
        var result = await _porterScoreRepository.FindByPorterGroupIdAsync(porterGroupId, pageRequest, ct);

        return new SearchDto.Builder().Content(result.Items)
            .TotalElements(result.TotalElements)
            .TotalPages(result.TotalPages)
            .CurrentPage(page).PageSize(size).Build();
    }

    public Task<List<PorterScoreDto>> GetAllByPorterGroupIdAsync(long? porterGroupId, CancellationToken ct = default) =>
        _porterScoreRepository.FindByPorterGroupIdAsync(porterGroupId, ct);

    public Task<List<PorterScoreDto>> GetKeyOpportunityByPorterGroupAsync(string? body, CancellationToken ct = default)
    {
        var json = PorterJsonHelper.Parse(body);
        var porterGroupId = PorterJsonHelper.OptLong(json, "porterGroupId");
        var planId = PorterJsonHelper.OptLong(json, "planId");
        return _porterScoreRepository.GetKeyOpportunityByPorterGroupAsync(porterGroupId, planId, ct);
    }

    public Task<List<PorterScoreDto>> GetKeyThreatByPorterGroupAsync(string? body, CancellationToken ct = default)
    {
        var json = PorterJsonHelper.Parse(body);
        var porterGroupId = PorterJsonHelper.OptLong(json, "porterGroupId");
        var planId = PorterJsonHelper.OptLong(json, "planId");
        return _porterScoreRepository.GetKeyThreatByPorterGroupAsync(porterGroupId, planId, ct);
    }

    public Task<List<PorterScoreDto>> GetAllByPorterGroupAndProductAndMarketAsync(string? body, CancellationToken ct = default)
    {
        var json = PorterJsonHelper.Parse(body);
        var porterGroupId = PorterJsonHelper.OptLong(json, "porterGroupId");
        var marketId = PorterJsonHelper.OptLong(json, "marketId");
        var productId = PorterJsonHelper.OptLong(json, "productId");
        return _porterScoreRepository.GetAllByPorterGroupAndProductAndMarketAsync(porterGroupId, marketId, productId, ct);
    }

    public async Task<PorterScore> MapToEntityAsync(PorterScoreDto porterScoreDto, CancellationToken ct = default)
    {
        var porterScore = new PorterScore
        {
            Id = porterScoreDto.Id ?? 0,
            Title = porterScoreDto.Title ?? string.Empty
        };

        if (porterScoreDto.PorterGroupDto != null)
        {
            var porterGroup = await _porterGroupService.FindByIdOrElseThrowAsync(porterScoreDto.PorterGroupDto.Id, ct);
            porterScore.PorterGroup = porterGroup;
            porterScore.PorterGroupId = porterGroup.Id;
        }

        if (porterScoreDto.PlanId != null)
        {
            var planPeriod = await _planPeriodService.FindPlanPeriodByPlanIdAsync(porterScoreDto.PlanId.Value, ct);
            porterScore.PlanPeriod = planPeriod;
            porterScore.PlanId = planPeriod.PlanId;
        }

        if (porterScoreDto.MarketDto != null)
        {
            var market = await _marketService.FindByIdOrElseThrowAsync(porterScoreDto.MarketDto.Id, ct);
            porterScore.Market = market;
            porterScore.MarketId = market.Id;
        }

        if (porterScoreDto.ProductDto != null)
        {
            var product = await _productService.FindByIdOrElseThrowAsync(porterScoreDto.ProductDto.Id, ct);
            porterScore.Product = product;
            porterScore.ProductId = product.Id;
        }

        porterScore.CurrentScore = porterScoreDto.CurrentScore;
        porterScore.FutureScore = porterScoreDto.FutureScore;
        porterScore.Opportunity = porterScoreDto.Opportunity ?? false;
        return porterScore;
    }
}
