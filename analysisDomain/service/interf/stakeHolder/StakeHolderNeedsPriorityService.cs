using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.stakeHolder;
using SemBackDotNet10.analysisDomain.model.entity.stakeHolder;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.interf.stakeHolder;

public interface IStakeHolderNeedsPriorityService
{
    Task<StakeholderNeedsPriority> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task<List<StakeHolderNeedsPriorityDto>> CreateNewStakeHolderNeedsPriorityAsync(List<StakeHolderNeedsPriorityDto> dtoList, CancellationToken ct = default);

    Task<StakeHolderNeedsPriorityDto> GetStakeHolderNeedsPriorityByIdAsync(long id, CancellationToken ct = default);

    Task DeleteStakeHolderNeedsPriorityByIdAsync(List<long> ids, CancellationToken ct = default);

    Task<List<StakeHolderNeedsPriorityDto>> EditStakeHolderNeedsPriorityByIdAsync(List<StakeHolderNeedsPriorityDto> dtoList, CancellationToken ct = default);

    Task<SearchDto> GetStakeHolderNeedsPriorityAsync(string body, int page, int size, string? orderBy, SortDirection? direction, CancellationToken ct = default);

    Task<List<StakeHolderNeedsPriorityDto>?> GetStakeHolderNeedsPriorityListAsync(string body, CancellationToken ct = default);

    Task<List<StakeHolderNeedAnswerDto>> GetStakeHolderNeedsAnswerByStakeHolderGroupIdAsync(string body, CancellationToken ct = default);
}
