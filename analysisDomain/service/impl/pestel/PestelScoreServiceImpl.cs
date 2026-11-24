using System.Text.Json;
using Common.exception;
using Common.utils;
using SemBackDotNet10.analysisDomain.model.dto.pestel;
using SemBackDotNet10.analysisDomain.model.entity.pestel;
using SemBackDotNet10.analysisDomain.repository.pestel;
using SemBackDotNet10.analysisDomain.service.interf.pestel;
using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;
using global::Common.logging;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.impl.pestel;

public class PestelScoreServiceImpl : IPestelScoreService
{
    private readonly IPestelScoreRepository _pestelScoreRepository;
    private readonly IPestelService _pestelService;
    private readonly global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository _planPeriodService;
    private readonly SemLogger _semLogger;

    public PestelScoreServiceImpl(IPestelScoreRepository pestelScoreRepository, IPestelService pestelService,
        global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository planPeriodService, SemLogger semLogger)
    {
        _pestelScoreRepository = pestelScoreRepository;
        _pestelService = pestelService;
        _planPeriodService = planPeriodService;
        _semLogger = semLogger;
    }

    public async Task<PestelScore> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.PestelScore.idIsNotValid");
        return await _pestelScoreRepository.FindByIdAsync(id.Value, ct)
               ?? throw new BusinessException("error.PestelScore.idIsNotValid");
    }

    public async Task CreateNewPestelScoreAsync(PestelScoreDto pestelScoreDto, CancellationToken ct = default)
    {
        var pestelScore = await MapToEntityAsync(pestelScoreDto, ct);
        if (pestelScore.Id != 0)
            await _pestelScoreRepository.UpdateAsync(pestelScore, ct);
        else
            await _pestelScoreRepository.AddAsync(pestelScore, ct);
    }

    public async Task DeletePestelScoreAsync(long? pestelScoreId, CancellationToken ct = default)
    {
        var pestelScore = await FindByIdOrElseThrowAsync(pestelScoreId, ct);
        await _pestelScoreRepository.DeleteAsync(pestelScore, ct);
    }

    public async Task EditPestelScoreAsync(PestelScoreDto pestelScoreDto, CancellationToken ct = default)
    {
        var pestelScore = await FindByIdOrElseThrowAsync(pestelScoreDto.Id, ct);
        pestelScore.OpportunityOrThreat = pestelScoreDto.OpportunityOrThreat ?? string.Empty;
        if (pestelScoreDto.PestelDto?.Id != null)
        {
            var pestel = await _pestelService.FindByIdOrElseThrowAsync(pestelScoreDto.PestelDto.Id, ct);
            pestelScore.Pestel = pestel;
            pestelScore.PestelId = pestel.Id;
        }
        if (pestelScoreDto.PlanId != null)
        {
            var planPeriod = await _planPeriodService.FindPlanPeriodByPlanIdAsync(pestelScoreDto.PlanId.Value, ct);
            pestelScore.PlanPeriod = planPeriod;
            pestelScore.PlanId = planPeriod.PlanId;
        }
        pestelScore.CurrentScore = pestelScoreDto.CurrentScore;
        pestelScore.FutureScore = pestelScoreDto.FutureScore;
        pestelScore.PreviousYearCurrentScore = pestelScoreDto.PreviousYearCurrentScore;
        pestelScore.PreviousYearFutureScore = pestelScoreDto.PreviousYearFutureScore;
        await _pestelScoreRepository.UpdateAsync(pestelScore, ct);
    }

    public async Task<SearchDto> GetAllByPestelGroupIdAsync(long? pestelGroupId, int page, int size, string? orderBy, SortDirection? direction, CancellationToken ct = default)
    {
        var pageRequest = PageUtils.GetInstance(page, size, orderBy, direction);
        var result = await _pestelScoreRepository.FindByPestelGroupIdAsync(pestelGroupId, pageRequest, ct);
        return new SearchDto.Builder()
            .Content(result.Items)
            .TotalElements(result.TotalElements)
            .TotalPages(result.TotalPages)
            .CurrentPage(page)
            .PageSize(size)
            .Build();
    }

    public Task<List<PestelScoreDto>> GetKeyOpportunityByPestelGroupAsync(string body, CancellationToken ct = default)
    {
        var (pestelGroupId, planId) = ReadGroupAndPlan(body);
        return _pestelScoreRepository.GetKeyOpportunityByPestelGroupAsync(pestelGroupId, planId, ct);
    }

    public Task<List<PestelScoreDto>> GetKeyThreatByPestelGroupAsync(string body, CancellationToken ct = default)
    {
        var (pestelGroupId, planId) = ReadGroupAndPlan(body);
        return _pestelScoreRepository.GetKeyThreatByPestelGroupAsync(pestelGroupId, planId, ct);
    }

    public async Task<PestelScore> MapToEntityAsync(PestelScoreDto dto, CancellationToken ct = default)
    {
        var pestelScore = new PestelScore
        {
            Id = dto.Id ?? 0,
            OpportunityOrThreat = dto.OpportunityOrThreat ?? string.Empty,
            PreviousYearCurrentScore = dto.PreviousYearCurrentScore,
            PreviousYearFutureScore = dto.PreviousYearFutureScore,
            CurrentScore = dto.CurrentScore,
            FutureScore = dto.FutureScore
        };
        if (dto.PestelDto != null)
        {
            var pestel = await _pestelService.FindByIdOrElseThrowAsync(dto.PestelDto.Id, ct);
            pestelScore.Pestel = pestel;
            pestelScore.PestelId = pestel.Id;
        }
        if (dto.PlanId != null)
        {
            var planPeriod = await _planPeriodService.FindPlanPeriodByPlanIdAsync(dto.PlanId.Value, ct);
            pestelScore.PlanPeriod = planPeriod;
            pestelScore.PlanId = planPeriod.PlanId;
        }
        return pestelScore;
    }

    private static (long? PestelGroupId, long? PlanId) ReadGroupAndPlan(string body)
    {
        using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(body) ? "{}" : body);
        return (ReadLong(doc.RootElement, "pestelGroupId"), ReadLong(doc.RootElement, "planId"));
    }

    private static long? ReadLong(JsonElement root, string key)
    {
        if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty(key, out var value))
            return null;
        return value.ValueKind switch
        {
            JsonValueKind.Number => value.GetInt64(),
            JsonValueKind.String when long.TryParse(value.GetString(), out var parsed) => parsed,
            _ => null
        };
    }
}
