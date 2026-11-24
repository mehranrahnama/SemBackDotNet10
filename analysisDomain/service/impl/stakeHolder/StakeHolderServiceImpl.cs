using PageUtils = global::Common.utils.PageUtils;
using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using Common.exception;
using SemBackDotNet10.analysisDomain.model.dto.stakeHolder;
using SemBackDotNet10.analysisDomain.model.entity.enums;
using SemBackDotNet10.analysisDomain.model.entity.stakeHolder;
using SemBackDotNet10.analysisDomain.repository.stakeHolder;
using SemBackDotNet10.analysisDomain.service.impl.resource;
using SemBackDotNet10.analysisDomain.service.interf.stakeHolder;
using SemBackDotNet10.Common.exception;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.impl.stakeHolder;

public class StakeHolderServiceImpl : IStakeHolderService
{
    private readonly IStakeHolderRepository _stakeHolderRepository;
    private readonly IStakeHolderGroupService _stakeHolderGroupService;

    public StakeHolderServiceImpl(IStakeHolderRepository stakeHolderRepository, IStakeHolderGroupService stakeHolderGroupService)
    {
        _stakeHolderRepository = stakeHolderRepository;
        _stakeHolderGroupService = stakeHolderGroupService;
    }

    public async Task<StakeHolder> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.StakeHolder.idIsNotValid");
        return await _stakeHolderRepository.FindByIdAsync(id.Value, ct)
            ?? throw new BusinessException("error.StakeHolder.idIsNotValid");
    }

    public async Task<SearchDto> FindAllStakeHoldersByStakeHolderGroupIdAsync(long? groupId, string? filter, long? planId, int page, int size, string? orderBy, SortDirection? direction, CancellationToken ct = default)
    {
        var filterParam = filter ?? "";
        var pageable = PageUtils.GetInstance(page, size, orderBy, direction);
        var result = await _stakeHolderRepository.FindAllStakeHoldersByStakeHolderGroupIdAsync(pageable, filterParam, planId, groupId, ct);
        return new SearchDto.Builder()
            .Content(result.Items)
            .CurrentPage(page)
            .PageSize(size)
            .TotalPages(result.TotalPages)
            .TotalElements(result.TotalElements)
            .Build();
    }

    public async Task CreateStakeHolderListAsync(List<StakeHolderDto> dtoList, CancellationToken ct = default)
    {
        if (dtoList == null || dtoList.Count == 0)
            throw new BusinessException("error.CreateStakeHolder.empty");

        var stakeHolders = new List<StakeHolder>();
        foreach (var dto in dtoList)
        {
            var group = await _stakeHolderGroupService.FindByIdOrElseThrowAsync(dto.StakeholderGroupId, ct);
            stakeHolders.Add(new StakeHolder
            {
                StakeHolderTitle = dto.StakeholderTitle ?? string.Empty,
                StakeHolderGroup = group,
                StakeHolderGroupId = group.Id
            });
        }
        await _stakeHolderRepository.AddRangeAsync(stakeHolders, ct);
    }

    public async Task<StakeHolder> UpdateStakeHolderAsync(StakeHolderDto dto, long id, CancellationToken ct = default)
    {
        var found = await FindByIdOrElseThrowAsync(id, ct);
        if (dto.StakeholderGroupId != null && found.StakeHolderGroupId != dto.StakeholderGroupId)
        {
            var group = await _stakeHolderGroupService.FindByIdOrElseThrowAsync(dto.StakeholderGroupId, ct);
            found.StakeHolderGroup = group;
            found.StakeHolderGroupId = group.Id;
        }
        found.StakeHolderTitle = dto.StakeholderTitle ?? string.Empty;
        await _stakeHolderRepository.UpdateAsync(found, ct);
        return found;
    }

    public async Task DeleteStakeHolderByIdAsync(long id, CancellationToken ct = default)
    {
        var stakeHolder = await FindByIdOrElseThrowAsync(id, ct);
        await _stakeHolderRepository.DeleteAsync(stakeHolder, ct);
    }

    public async Task SetStakeHolderPriorityAsync(StakeHolderDto stakeHolderDto, CancellationToken ct = default)
    {
        var stakeHolder = await FindByIdOrElseThrowAsync(stakeHolderDto.StakeHolderId, ct);

        stakeHolder.Interest = stakeHolderDto.Interest;
        stakeHolder.Power = stakeHolderDto.Power;
        stakeHolder.BehaviorStrategy = FindProperBehaviorStrategy(stakeHolderDto);

        await _stakeHolderRepository.UpdateAsync(stakeHolder, ct);
    }

    public Task<List<StakeHolderDto>> FindAllStakeHoldersByPlanIdWithPriorityAsync(string body, CancellationToken ct = default)
    {
        var json = AnalysisJsonHelper.Parse(body);
        var planId = AnalysisJsonHelper.OptLong(json, "planId");
        return _stakeHolderRepository.FindAllStakeHoldersByPlanIdWithPriorityAsync(planId, ct);
    }

    public async Task<StakeHolder> UpdatePowerInterestAsync(StakeHolderDto dto, CancellationToken ct = default)
    {
        var found = await FindByIdOrElseThrowAsync(dto.StakeHolderId, ct);

        found.Power = dto.Power;
        found.Interest = dto.Interest;
        found.BehaviorStrategy = FindProperBehaviorStrategy(dto);
        await _stakeHolderRepository.UpdateAsync(found, ct);
        return found;
    }

    public async Task<StakeHolder> DeletePowerInterestAsync(StakeHolderDto dto, CancellationToken ct = default)
    {
        var found = await FindByIdOrElseThrowAsync(dto.StakeHolderId, ct);

        found.Power = null;
        found.Interest = null;
        found.BehaviorStrategy = null;
        await _stakeHolderRepository.UpdateAsync(found, ct);
        return found;
    }

    internal static BehaviorStrategy? FindProperBehaviorStrategy(StakeHolderDto dto)
    {
        var power = dto.Power ?? 0;
        var interest = dto.Interest ?? 0;

        if (power >= 1 && power <= 5)
        {
            if (interest >= 1 && interest <= 5)
                return BehaviorStrategy.Monitoring;
            if (interest >= 6 && interest <= 10)
                return BehaviorStrategy.Informing;
        }
        else if (power >= 6 && power <= 10)
        {
            if (interest >= 1 && interest <= 5)
                return BehaviorStrategy.AchievingSatisfaction;
            if (interest >= 6 && interest <= 10)
                return BehaviorStrategy.EffectiveManagement;
        }

        return null;
    }
}
