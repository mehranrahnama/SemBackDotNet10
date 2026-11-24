using PageUtils = global::Common.utils.PageUtils;
using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using SemBackDotNet10.analysisDomain.repository.porter;
using SemBackDotNet10.analysisDomain.service.interf.porter;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.Common.exception;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.impl.porter;

public class MarketServiceImpl : IMarketService
{
    private readonly IMarketRepository _marketRepository;
    private readonly global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository _planPeriodService;

    public MarketServiceImpl(IMarketRepository marketRepository, global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository planPeriodService)
    {
        _marketRepository = marketRepository;
        _planPeriodService = planPeriodService;
    }

    public IMarketRepository GetRepository() => _marketRepository;

    public async Task<Market> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.Market.idIsNull");

        return await _marketRepository.FindByIdAsync(id.Value, ct)
            ?? throw new BusinessException("error.Market.idIsNotValid");
    }

    public async Task<MarketDto> CreateMarketAsync(MarketDto marketDto, CancellationToken ct = default)
    {
        if (marketDto == null)
            throw new BusinessException("error.market.isNull");

        var entity = await MapToEntityAsync(marketDto, ct);
        if (entity.Id == 0)
            await _marketRepository.AddAsync(entity, ct);
        else
            await _marketRepository.UpdateAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task<SearchDto> FindAllPageAsync(int page, int size, string? orderBy, SortDirection? direction, string? body, CancellationToken ct = default)
    {
        var json = PorterJsonHelper.Parse(body);
        var filter = PorterJsonHelper.GetString(json, "filter");
        var planId = PorterJsonHelper.OptLong(json, "planId");
        var pageRequest = PageUtils.GetInstance(page, size, orderBy, direction);
        var result = await _marketRepository.FindAllPageAsync(filter, planId, pageRequest, ct);
        return new SearchDto.Builder()
            .PageSize(size).CurrentPage(page).TotalPages(result.TotalPages)
            .TotalElements(result.TotalElements)
            .Content(result.Items)
            .Build();
    }

    public async Task<List<MarketDto>> FindAllMarketAsync(CancellationToken ct = default)
    {
        var markets = await _marketRepository.Query.ToListAsync(ct);
        return markets.Select(MapToDto).ToList();
    }

    public async Task<MarketDto> EditMarketAsync(MarketDto marketDto, CancellationToken ct = default)
    {
        var result = await FindByIdOrElseThrowAsync(marketDto.Id, ct);
        result.Title = marketDto.Title ?? string.Empty;
        await _marketRepository.UpdateAsync(result, ct);
        return MapToDto(result);
    }

    public async Task DeleteMarketAsync(long? id, CancellationToken ct = default)
    {
        var result = await FindByIdOrElseThrowAsync(id, ct);
        await _marketRepository.DeleteAsync(result, ct);
    }

    public async Task<Market> MapToEntityAsync(MarketDto marketDto, CancellationToken ct = default)
    {
        var planPeriod = await _planPeriodService.FindPlanPeriodByPlanIdAsync(
            marketDto.PlanId ?? throw new BusinessException("error.planPeriod.notFound"), ct);
        return new Market
        {
            Id = marketDto.Id ?? 0,
            Title = marketDto.Title ?? string.Empty,
            PlanPeriod = planPeriod,
            PlanId = planPeriod.PlanId
        };
    }

    public MarketDto MapToDto(Market market) =>
        new()
        {
            Id = market.Id,
            Title = market.Title,
            PlanId = market.PlanId
        };
}
