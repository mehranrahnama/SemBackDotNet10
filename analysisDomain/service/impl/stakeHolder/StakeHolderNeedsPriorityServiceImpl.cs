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
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.Common.exception;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.impl.stakeHolder;

public class StakeHolderNeedsPriorityServiceImpl : IStakeHolderNeedsPriorityService
{
    private readonly string _tenantName;
    private readonly IStakeHolderNeedsPriorityRepository _stakeHolderNeedsPriorityRepository;
    private readonly IStakeHolderService _stakeHolderService;

    public StakeHolderNeedsPriorityServiceImpl(
        IConfiguration configuration,
        IStakeHolderNeedsPriorityRepository stakeHolderNeedsPriorityRepository,
        IStakeHolderService stakeHolderService)
    {
        _tenantName = configuration["Security:DefaultTenantName"]
            ?? throw new InvalidOperationException("Security:DefaultTenantName is not configured.");
        _stakeHolderNeedsPriorityRepository = stakeHolderNeedsPriorityRepository;
        _stakeHolderService = stakeHolderService;
    }

    private string TenantName => _tenantName;

    public async Task<List<StakeHolderNeedsPriorityDto>> CreateNewStakeHolderNeedsPriorityAsync(List<StakeHolderNeedsPriorityDto> dtoList, CancellationToken ct = default)
    {
        try
        {
            var savingList = new List<StakeholderNeedsPriority>();
            foreach (var dto in dtoList)
            {
                var stakeHolder = await _stakeHolderService.FindByIdOrElseThrowAsync(dto.StakeHolderId, ct);
                if (stakeHolder.BehaviorStrategy == null)
                    throw new BusinessException("ابتدا اولویت بندی ذینفع را مشخص کنید.");
                savingList.Add(new StakeholderNeedsPriority
                {
                    NeedTitle = dto.NeedTitle ?? string.Empty,
                    Importance = dto.Importance ?? 0,
                    Impact = dto.Impact ?? 0,
                    StakeHolder = stakeHolder,
                    StakeHolderId = stakeHolder.Id,
                    AnsweringType = FindProperAnsweringType(dto, stakeHolder)
                });
            }
            await _stakeHolderNeedsPriorityRepository.AddRangeAsync(savingList, ct);
            return savingList.Select(MapEntityToDto).ToList();
        }
        catch (Exception e)
        {
            throw new BusinessException(e.Message);
        }
    }

    public async Task<StakeHolderNeedsPriorityDto> GetStakeHolderNeedsPriorityByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await FindByIdOrElseThrowAsync(id, ct);
        entity.StakeHolder ??= await _stakeHolderService.FindByIdOrElseThrowAsync(entity.StakeHolderId, ct);
        return MapEntityToDto(entity);
    }

    public async Task DeleteStakeHolderNeedsPriorityByIdAsync(List<long> ids, CancellationToken ct = default)
    {
        try
        {
            var deletingList = new List<StakeholderNeedsPriority>();
            foreach (var id in ids)
                deletingList.Add(await FindByIdOrElseThrowAsync(id, ct));
            await _stakeHolderNeedsPriorityRepository.DeleteRangeAsync(deletingList, ct);
        }
        catch (Exception)
        {
            throw new BusinessException("error.stakeHolderNeedsPriority.idIsNotValid");
        }
    }

    public async Task<List<StakeHolderNeedsPriorityDto>> EditStakeHolderNeedsPriorityByIdAsync(List<StakeHolderNeedsPriorityDto> dtoList, CancellationToken ct = default)
    {
        var updatingList = new List<StakeholderNeedsPriority>();
        foreach (var dto in dtoList)
        {
            var entity = await FindByIdOrElseThrowAsync(dto.StakeHolderNeedsPriorityId, ct);
            var stakeHolder = await _stakeHolderService.FindByIdOrElseThrowAsync(dto.StakeHolderId, ct);
            entity.StakeHolder = stakeHolder;
            entity.StakeHolderId = stakeHolder.Id;
            entity.AnsweringType = FindProperAnsweringType(dto, stakeHolder);
            entity.NeedTitle = dto.NeedTitle ?? string.Empty;
            entity.Importance = dto.Importance ?? 0;
            entity.Impact = dto.Impact ?? 0;
            updatingList.Add(entity);
        }
        await _stakeHolderNeedsPriorityRepository.UpdateRangeAsync(updatingList, ct);
        return updatingList.Select(MapEntityToDto).ToList();
    }

    public async Task<SearchDto> GetStakeHolderNeedsPriorityAsync(string body, int page, int size, string? orderBy, SortDirection? direction, CancellationToken ct = default)
    {
        var json = AnalysisJsonHelper.Parse(body);
        var stakeHolderId = AnalysisJsonHelper.OptLong(json, "stakeHolderId");
        var filter = AnalysisJsonHelper.OptString(json, "filter");
        var pageable = PageUtils.GetInstance(page, size, orderBy, direction);

        var result = await _stakeHolderNeedsPriorityRepository.FindAllStakeHolderNeedsPriorityPageAsync(stakeHolderId, filter, pageable, ct);

        return new SearchDto.Builder()
            .PageSize(size)
            .CurrentPage(page)
            .TotalElements(result.TotalElements)
            .TotalPages(result.TotalPages)
            .Content(result.Items)
            .Build();
    }

    public async Task<List<StakeHolderNeedsPriorityDto>?> GetStakeHolderNeedsPriorityListAsync(string body, CancellationToken ct = default)
    {
        var json = AnalysisJsonHelper.Parse(body);
        var stakeHolderId = AnalysisJsonHelper.OptLong(json, "stakeHolderId");
        var filter = AnalysisJsonHelper.OptString(json, "filter");
        var result = await _stakeHolderNeedsPriorityRepository.FindAllStakeHolderNeedsPriorityListAsync(stakeHolderId, filter, ct);
        return result.Count > 0 ? result : null;
    }

    public async Task<StakeholderNeedsPriority> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.StakeHolderNeedsPriority.idIsNotValid");
        return await _stakeHolderNeedsPriorityRepository.FindByIdAsync(id.Value, ct)
            ?? throw new BusinessException("error.StakeHolderNeedsPriority.idIsNotValid");
    }

    public async Task<List<StakeHolderNeedAnswerDto>> GetStakeHolderNeedsAnswerByStakeHolderGroupIdAsync(string body, CancellationToken ct = default)
    {
        var json = AnalysisJsonHelper.Parse(body);
        var stakeHolderGroupId = AnalysisJsonHelper.GetLong(json, "stakeHolderGroupId");
        return await _stakeHolderNeedsPriorityRepository.GetStakeHolderNeedsAnswerByStakeHolderGroupIdAsync(stakeHolderGroupId, ct);
    }

    private static StakeHolderNeedsPriorityDto MapEntityToDto(StakeholderNeedsPriority e) =>
        new()
        {
            StakeHolderNeedsPriorityId = e.Id,
            StakeHolderId = e.StakeHolder?.Id ?? e.StakeHolderId,
            NeedTitle = e.NeedTitle,
            Importance = e.Importance,
            Impact = e.Impact,
            StakeHolderName = e.StakeHolder?.StakeHolderTitle
        };

    internal AnsweringType? FindProperAnsweringType(StakeHolderNeedsPriorityDto need, StakeHolder stakeHolder)
    {
        switch (stakeHolder.BehaviorStrategy)
        {
            case BehaviorStrategy.EffectiveManagement: // مدیریت موثر
            case BehaviorStrategy.AchievingSatisfaction: // کسب رضایت
                if (IsVeryImportant(need)) return AnsweringType.BusinessLevel;
                if (IsImportant(need)) return AnsweringType.TaskLevel;
                if (IsModerate(need)) return AnsweringType.CurrentActivity;
                break;
            case BehaviorStrategy.Informing: // اطلاع رسانی
                if (IsVeryImportant(need)) return AnsweringType.TaskLevel;
                if (IsImportant(need)) return AnsweringType.CurrentActivity;
                if (IsModerate(need)) return AnsweringType.CurrentActivity;
                break;
            case BehaviorStrategy.Monitoring: // پایش/مراقبت
                if (IsVeryImportant(need) || IsImportant(need) || IsModerate(need))
                    return AnsweringType.CurrentActivity;
                break;
        }
        return null;
    }

    private bool IsVeryImportant(StakeHolderNeedsPriorityDto need)
    {
        var impact = need.Impact.GetValueOrDefault();
        var importance = need.Importance.GetValueOrDefault();
        if (TenantName == "HOSCO")
        {
            if ((impact >= 2 && impact <= 3) && (importance >= 4 && importance <= 5))
                return true;
        }
        else if (TenantName == "IRISA")
        {
            if ((impact >= 4 && impact <= 5) && (importance >= 4 && importance <= 5))
                return true;
        }
        return false;
    }

    private bool IsImportant(StakeHolderNeedsPriorityDto need)
    {
        var impact = need.Impact.GetValueOrDefault();
        var importance = need.Importance.GetValueOrDefault();
        if (TenantName == "HOSCO")
        {
            if (((impact >= 2 && impact <= 3) && (importance >= 2 && importance <= 3)) ||
                ((impact >= -1 && impact <= 1) && (importance >= 4 && importance <= 5)) ||
                (impact == 1 && (importance >= 2 && importance <= 3)) ||
                ((impact >= -3 && impact <= -2) && (importance >= 3 && importance <= 5)))
                return true;
        }
        else if (TenantName == "IRISA")
        {
            if (((impact >= 4 && impact <= 5) && importance == 3) ||
                (impact == 5 && (importance >= 1 && importance <= 2)) ||
                ((impact >= 1 && impact <= 3) && importance == 5) ||
                (impact == 3 && importance == 4))
                return true;
        }
        return false;
    }

    private bool IsModerate(StakeHolderNeedsPriorityDto need)
    {
        var impact = need.Impact.GetValueOrDefault();
        var importance = need.Importance.GetValueOrDefault();
        if (TenantName == "HOSCO")
        {
            if (((impact >= 2 && impact <= 3) && importance == 1) ||
                (impact == -1 && (importance >= 1 && importance <= 3)))
                return true;
        }
        else if (TenantName == "IRISA")
        {
            if ((impact == 4 && (importance >= 1 && importance <= 2)) ||
                ((impact >= 1 && impact <= 2) && importance == 4) ||
                ((impact >= 2 && impact <= 3) && importance == 3) ||
                (impact == 3 && importance == 2))
                return true;
        }
        return false;
    }
}
