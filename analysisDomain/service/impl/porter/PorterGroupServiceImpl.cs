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

public class PorterGroupServiceImpl : IPorterGroupService
{
    private readonly IPorterGroupRepository _porterGroupRepository;
    private readonly global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository _planPeriodService;
    private readonly IPorterRepository _porterRepository;

    // به‌جای IPorterService از ریپازیتوری استفاده می‌شود تا وابستگی دوری DI ایجاد نشود (در جاوا @Lazy بود)
    public PorterGroupServiceImpl(IPorterGroupRepository porterGroupRepository,
        global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository planPeriodService,
        IPorterRepository porterRepository)
    {
        _porterGroupRepository = porterGroupRepository;
        _planPeriodService = planPeriodService;
        _porterRepository = porterRepository;
    }

    public IPorterGroupRepository GetRepository() => _porterGroupRepository;

    public async Task<PorterGroup> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.PorterGroup.idIsNull");

        return await _porterGroupRepository.FindByIdAsync(id.Value, ct)
            ?? throw new BusinessException("error.PorterGroup.idIsNotValid");
    }

    public async Task<PorterGroupDto> CreatePorterGroupAsync(PorterGroupDto porterGroupDto, CancellationToken ct = default)
    {
        if (porterGroupDto == null)
            throw new BusinessException("error.PorterGroup.isNull");

        var entity = await MapToEntityAsync(porterGroupDto, ct);
        if (entity.Id == 0)
            await _porterGroupRepository.AddAsync(entity, ct);
        else
            await _porterGroupRepository.UpdateAsync(entity, ct);
        return MapToDto(entity);
    }

    public async Task<SearchDto> FindAllPageAsync(int page, int size, string? orderBy, SortDirection? direction, string? body, CancellationToken ct = default)
    {
        var json = PorterJsonHelper.Parse(body);
        var filter = PorterJsonHelper.GetString(json, "filter");
        var planId = PorterJsonHelper.OptLong(json, "planId");
        var pageRequest = PageUtils.GetInstance(page, size, orderBy, direction);
        var result = await _porterGroupRepository.FindAllPageAsync(filter, planId, pageRequest, ct);
        return new SearchDto.Builder()
            .PageSize(size).CurrentPage(page).TotalPages(result.TotalPages)
            .TotalElements(result.TotalElements)
            .Content(result.Items)
            .Build();
    }

    public async Task<List<PorterGroupDto>> FindAllPorterGroupsAsync(long? planId, CancellationToken ct = default)
    {
        var groups = await _porterGroupRepository.FindAllByPlanIdAsync(planId, ct);
        return groups.Select(MapToDto).ToList();
    }

    public async Task<PorterGroupDto> EditPorterGroupAsync(PorterGroupDto porterGroupDto, CancellationToken ct = default)
    {
        var result = await FindByIdOrElseThrowAsync(porterGroupDto.Id, ct);
        result.Title = porterGroupDto.Title ?? string.Empty;
        await _porterGroupRepository.UpdateAsync(result, ct);
        return MapToDto(result);
    }

    public async Task DeletePorterGroupAsync(long? id, CancellationToken ct = default)
    {
        var result = await FindByIdOrElseThrowAsync(id, ct);
        var porters = await _porterRepository.FindAllByPorterGroupIdAsync(result.Id, ct);
        if (porters.Count != 0)
            throw new BusinessException("ابتدا عوامل محیط صنعت را حذف کنید.");

        await _porterGroupRepository.DeleteAsync(result, ct);
    }

    public PorterGroupDto MapToDto(PorterGroup porterGroup) =>
        new()
        {
            Id = porterGroup.Id,
            Title = porterGroup.Title,
            PlanId = porterGroup.PlanId
        };

    public async Task<PorterGroup> MapToEntityAsync(PorterGroupDto porterGroupDto, CancellationToken ct = default)
    {
        var planPeriod = await _planPeriodService.FindPlanPeriodByPlanIdAsync(
            porterGroupDto.PlanId ?? throw new BusinessException("error.planPeriod.notFound"), ct);
        return new PorterGroup
        {
            Id = porterGroupDto.Id ?? 0,
            Title = porterGroupDto.Title ?? string.Empty,
            PlanPeriod = planPeriod,
            PlanId = planPeriod.PlanId
        };
    }
}
