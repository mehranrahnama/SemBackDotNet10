using PageUtils = global::Common.utils.PageUtils;
using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.stakeHolder;
using SemBackDotNet10.analysisDomain.model.entity.stakeHolder;
using SemBackDotNet10.analysisDomain.repository.stakeHolder;
using SemBackDotNet10.analysisDomain.service.impl.resource;
using SemBackDotNet10.analysisDomain.service.interf.stakeHolder;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.Common.exception;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.impl.stakeHolder;

public class StakeHolderGroupServiceImpl : IStakeHolderGroupService
{
    private readonly IStakeHolderGroupRepository _stakeHolderGroupRepository;
    // به‌جای IStakeHolderService (وابستگی حلقوی) مستقیماً ریپازیتوری ذینفع تزریق می‌شود
    private readonly IStakeHolderRepository _stakeHolderRepository;
    private readonly global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository _planPeriodService;

    public StakeHolderGroupServiceImpl(
        IStakeHolderGroupRepository stakeHolderGroupRepository,
        IStakeHolderRepository stakeHolderRepository,
        global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository planPeriodService)
    {
        _stakeHolderGroupRepository = stakeHolderGroupRepository;
        _stakeHolderRepository = stakeHolderRepository;
        _planPeriodService = planPeriodService;
    }

    public async Task<StakeHolderGroup> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.StakeHolderGroup.idIsNotValid");
        return await _stakeHolderGroupRepository.FindByIdAsync(id.Value, ct)
            ?? throw new BusinessException("error.StakeHolderGroup.idIsNotValid");
    }

    public async Task<SearchDto> GetAllStakeHolderGroupsAsync(string? filter, string? body, int page, int size, string? orderBy, SortDirection? direction, CancellationToken ct = default)
    {
        var filterParam = filter ?? "";
        var json = AnalysisJsonHelper.Parse(body);
        var planId = AnalysisJsonHelper.OptLong(json, "planId");
        var pageable = PageUtils.GetInstance(page, size, orderBy, direction);
        var result = await _stakeHolderGroupRepository.FindAllAsync(pageable, filterParam, planId, ct);

        return new SearchDto.Builder()
            .Content(result.Items)
            .CurrentPage(page)
            .PageSize(size)
            .TotalPages(result.TotalPages)
            .TotalElements(result.TotalElements)
            .Build();
    }

    public async Task CreateStakeHolderGroupsAsync(List<StakeHolderGroupDto> dtoList, CancellationToken ct = default)
    {
        if (dtoList == null || dtoList.Count == 0)
            throw new BusinessException("error.CreateStakeHolderGroup.empty");

        var groups = new List<StakeHolderGroup>();
        foreach (var dto in dtoList)
        {
            var planId = dto.PlanId ?? throw new BusinessException("error.PlanPeriod.planIdIsNull");
            var planPeriod = await _planPeriodService.FindPlanPeriodByPlanIdAsync(planId, ct);
            groups.Add(new StakeHolderGroup(dto.StakeHolderGroupTitle ?? string.Empty, planPeriod) { PlanId = planPeriod.PlanId });
        }
        await _stakeHolderGroupRepository.AddRangeAsync(groups, ct);
    }

    public async Task DeleteStakeHolderGroupByIdAsync(long id, CancellationToken ct = default)
    {
        var stakeHolders = await _stakeHolderRepository.FindStakeHolderByStakeHolderGroupIdAsync(id, ct);
        if (stakeHolders.Count > 0)
            throw new BusinessException("امکان حذف به دلیل وجود ذینفع وجود ندارد.");
        var stakeHolderGroup = await FindByIdOrElseThrowAsync(id, ct);
        await _stakeHolderGroupRepository.DeleteAsync(stakeHolderGroup, ct);
    }

    public async Task<StakeHolderGroup> UpdateStakeHolderGroupByIdAsync(long id, StakeHolderGroupDto dto, CancellationToken ct = default)
    {
        var found = await FindByIdOrElseThrowAsync(id, ct);
        found.StakeHolderGroupTitle = dto.StakeHolderGroupTitle ?? string.Empty;
        await _stakeHolderGroupRepository.UpdateAsync(found, ct);
        return found;
    }

    public Task<List<StakeHolderGroupDto>> GetAllStakeHolderGroupAsync(long planId, CancellationToken ct = default) =>
        _stakeHolderGroupRepository.FindAllByPlanIdAsync(planId, ct);
}
