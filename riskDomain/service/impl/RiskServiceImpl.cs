using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.riskDomain.domain;
using SemBackDotNet10.riskDomain.model.dto;
using SemBackDotNet10.riskDomain.model.entity;
using SemBackDotNet10.riskDomain.repository;
using SemBackDotNet10.riskDomain.service.interf;

namespace SemBackDotNet10.riskDomain.service.impl;

public class RiskServiceImpl : IRiskService
{
    private readonly IRiskRepository _riskRepository;
    private readonly IRiskActionService _riskActionService;
    private readonly IRiskMeasureService _riskMeasureService;
    private readonly IRiskDomainService _riskDomainService;
    private readonly SemDbContext _context;

    public RiskServiceImpl(
        IRiskRepository riskRepository,
        IRiskActionService riskActionService,
        IRiskMeasureService riskMeasureService,
        IRiskDomainService riskDomainService,
        SemDbContext context)
    {
        _riskRepository = riskRepository;
        _riskActionService = riskActionService;
        _riskMeasureService = riskMeasureService;
        _riskDomainService = riskDomainService;
        _context = context;
    }

    public IRiskRepository GetRepository() => _riskRepository;

    public async Task CreateRiskAsync(RiskDto riskDto, CancellationToken ct = default)
    {
        var risk = await GetRepository().AddAsync(await MapToEntityAsync(riskDto, ct), ct);

        foreach (var a in riskDto.RiskActionDtoList)
        {
            a.RiskId = risk.Id;
            await _riskActionService.CreateRiskActionAsync(a, ct);
        }

        foreach (var m in riskDto.RiskMeasureDtoList)
        {
            m.RiskId = risk.Id;
            await _riskMeasureService.CreateRiskMeasureAsync(m, ct);
        }
    }

    private async Task<Risk> MapToEntityAsync(RiskDto riskDto, CancellationToken ct)
    {
        var risk = new Risk
        {
            DesRisk = riskDto.DesRisk,
            Cause = riskDto.Cause,
            Effect = riskDto.Effect,
            Probability = riskDto.Probability,
            Severity = riskDto.Severity
        };
        var strategyObject = await FindStrategyObjectOrElseThrowAsync(riskDto.StrategyObjectDto?.Id, ct);
        risk.StrategyObject = strategyObject;
        risk.StrategyObjectId = strategyObject.Id;
        return risk;
    }

    private async Task<StrategyObject> FindStrategyObjectOrElseThrowAsync(long? id, CancellationToken ct)
    {
        if (id == null)
            throw new BusinessException("error.StrategyObject.idIsNull");
        return await _context.StrategyObjects.FindAsync(new object[] { id.Value }, ct)
               ?? throw new BusinessException("error.StrategyObject.idIsNotValid");
    }

    public async Task<SearchDto> GetRiskByPlanIdAndLevelIdAsync(string body, int page, int size, string? filter, string? orderBy, SortDirection? direction, CancellationToken ct = default)
    {
        using var json = JsonDocument.Parse(body);
        var planId = ReadNullableLong(json.RootElement, "planId");
        var levelId = ReadNullableLong(json.RootElement, "levelId");

        var pageRequest = global::Common.utils.PageUtils.GetInstance(page, size, orderBy, direction);

        var riskDtoList = await GetRepository().FindByPlanIdAndLevelIdAsync(planId, levelId, filter, ct);
        var riskIds = riskDtoList.Where(r => r.RiskId != null).Select(r => r.RiskId!.Value).ToList();

        var actionByRiskId = await GetRiskActionDtoAsync(riskIds, ct);
        var measureByRiskId = await GetRiskMeasureDtoAsync(riskIds, ct);

        foreach (var dto in riskDtoList)
        {
            var key = dto.RiskId ?? 0;
            dto.RiskActionDtoList = actionByRiskId.GetValueOrDefault(key) ?? new List<RiskActionDto>();
            dto.RiskMeasureDtoList = measureByRiskId.GetValueOrDefault(key) ?? new List<RiskMeasureDto>();
        }

        var start = Math.Min(pageRequest.Page * pageRequest.Size, riskDtoList.Count);
        var end = Math.Min(start + pageRequest.Size, riskDtoList.Count);
        var subListData = riskDtoList.GetRange(start, end - start);
        var totalPages = size == 0 ? 1 : (int)Math.Ceiling(riskDtoList.Count / (double)size);

        return new SearchDto.Builder()
            .Content(subListData)
            .TotalElements(riskDtoList.Count)
            .TotalPages(totalPages)
            .CurrentPage(page)
            .PageSize(size)
            .Build();
    }

    public async Task EditRiskAsync(RiskDto riskDto, CancellationToken ct = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        var risk = await _riskDomainService.FindByIdOrElseThrowAsync(riskDto.RiskId, ct);
        risk.DesRisk = riskDto.DesRisk;
        risk.Effect = riskDto.Effect;
        risk.Cause = riskDto.Cause;
        risk.Probability = riskDto.Probability;
        risk.Severity = riskDto.Severity;

        foreach (var a in riskDto.RiskActionDtoList)
            a.RiskId = risk.Id;
        foreach (var m in riskDto.RiskMeasureDtoList)
            m.RiskId = risk.Id;

        var strategyObject = await FindStrategyObjectOrElseThrowAsync(riskDto.StrategyObjectDto?.Id, ct);
        risk.StrategyObject = strategyObject;
        risk.StrategyObjectId = strategyObject.Id;

        await _riskMeasureService.EditRiskMeasureListAsync(riskDto, ct);
        await _riskActionService.EditRiskActionListAsync(riskDto, ct);

        foreach (var r in riskDto.RiskActionDtoList)
        {
            r.RiskId = risk.Id;
            await _riskActionService.EditRiskActionByIdAsync(r, ct);
        }

        await GetRepository().UpdateAsync(risk, ct);
        await transaction.CommitAsync(ct);
    }

    public async Task DeleteRiskAsync(long? riskId, CancellationToken ct = default)
    {
        await _riskActionService.DeleteByRiskIdAsync(riskId, ct);
        await _riskMeasureService.DeleteByRiskIdAsync(riskId, ct);
        var risk = await _riskDomainService.FindByIdOrElseThrowAsync(riskId, ct);
        await GetRepository().DeleteAsync(risk, ct);
    }

    private async Task<Dictionary<long, List<RiskActionDto>>> GetRiskActionDtoAsync(List<long> riskIds, CancellationToken ct)
    {
        var actions = await _riskActionService.GetRepository().FindByRiskIdInAsync(riskIds, ct);
        return actions.Where(a => a.RiskId != null)
            .GroupBy(a => a.RiskId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private async Task<Dictionary<long, List<RiskMeasureDto>>> GetRiskMeasureDtoAsync(List<long> riskIds, CancellationToken ct)
    {
        var measures = await _riskMeasureService.GetRepository().FindByRiskIdInAsync(riskIds, ct);
        return measures.Where(m => m.RiskId != null)
            .GroupBy(m => m.RiskId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private static long? ReadNullableLong(JsonElement root, string name) =>
        root.TryGetProperty(name, out var value) && value.ValueKind != JsonValueKind.Null
            ? value.GetInt64()
            : null;
}
