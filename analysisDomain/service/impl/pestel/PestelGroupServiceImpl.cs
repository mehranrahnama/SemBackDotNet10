using Common.exception;
using Common.logging;
using Common.utils;
using SemBackDotNet10.analysisDomain.model.dto.pestel;
using SemBackDotNet10.analysisDomain.model.entity.pestel;
using SemBackDotNet10.analysisDomain.repository.pestel;
using SemBackDotNet10.analysisDomain.service.interf.pestel;
using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;


namespace SemBackDotNet10.analysisDomain.service.impl.pestel;

public class PestelGroupServiceImpl : IPestelGroupService
{
    private readonly IPestelGroupRepository _pestelGroupRepository;
    private readonly global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository _planPeriodService;
    private readonly SemLogger _semLogger;

    public PestelGroupServiceImpl(IPestelGroupRepository pestelGroupRepository, global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository planPeriodService, SemLogger semLogger)
    {
        _pestelGroupRepository = pestelGroupRepository;
        _planPeriodService = planPeriodService;
        _semLogger = semLogger;
    }

    public async Task<PestelGroup> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.PestelGroup.idIsNull");
        return await _pestelGroupRepository.FindByIdAsync(id.Value, ct)
               ?? throw new BusinessException("error.PestelGroup.idIsNotValid");
    }

    public async Task<PestelGroupDto> CreateNewPestelGroupAsync(PestelGroupDto pestelGroup, CancellationToken ct = default)
    {
        if (pestelGroup == null)
            throw new BusinessException("error.PestelGroup.isNull");
        var entity = await MapToEntityAsync(pestelGroup, ct);
        if (entity.Id != 0)
            await _pestelGroupRepository.UpdateAsync(entity, ct);
        else
            await _pestelGroupRepository.AddAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task DeletePestelGroupByIdAsync(long? id, CancellationToken ct = default)
    {
        var res = await FindByIdOrElseThrowAsync(id, ct);
        await _pestelGroupRepository.DeleteAsync(res, ct);
    }

    public async Task<PestelGroupDto> EditPestelGroupAsync(PestelGroupDto pestelGroup, CancellationToken ct = default)
    {
        var res = await FindByIdOrElseThrowAsync(pestelGroup.Id, ct);
        res.Title = pestelGroup.Title ?? string.Empty;
        await _pestelGroupRepository.UpdateAsync(res, ct);
        return MapToDto(res);
    }

    public async Task<SearchDto> FindAllPageAsync(int page, int size, string? orderBy, SortDirection? direction, PestelGroupDto? dto, CancellationToken ct = default)
    {
        var pageRequest = PageUtils.GetInstance(page, size, orderBy, direction);
        var res = await _pestelGroupRepository.FindAllPageAsync(dto?.Title, dto?.PlanId, pageRequest, ct);
        return new SearchDto.Builder()
            .PageSize(size)
            .CurrentPage(page)
            .TotalPages(res.TotalPages)
            .TotalElements(res.TotalElements)
            .Content(res.Items)
            .Build();
    }

    public async Task<List<PestelGroupDto>> FindAllPestelGroupsAsync(long? planId, CancellationToken ct = default)
    {
        var groups = await _pestelGroupRepository.FindAllByPlanIdAsync(planId, ct);
        return groups.Select(MapToDto).ToList();
    }

    private static PestelGroupDto MapToDto(PestelGroup res) => new()
    {
        Id = res.Id,
        Title = res.Title,
        PlanId = res.PlanId
    };

    private async Task<PestelGroup> MapToEntityAsync(PestelGroupDto dto, CancellationToken ct)
    {
        var entity = new PestelGroup
        {
            Id = dto.Id ?? 0,
            Title = dto.Title ?? string.Empty
        };
        if (dto.PlanId != null)
        {
            var planPeriod = await _planPeriodService.FindPlanPeriodByPlanIdAsync(dto.PlanId.Value, ct);
            entity.PlanPeriod = planPeriod;
            entity.PlanId = planPeriod.PlanId;
        }
        return entity;
    }
}
