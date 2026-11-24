using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.stakeHolder;
using SemBackDotNet10.analysisDomain.model.entity.stakeHolder;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.interf.stakeHolder;

public interface IStakeHolderService
{
    Task<StakeHolder> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task<SearchDto> FindAllStakeHoldersByStakeHolderGroupIdAsync(long? groupId, string? filter, long? planId, int page, int size, string? orderBy, SortDirection? direction, CancellationToken ct = default);

    Task CreateStakeHolderListAsync(List<StakeHolderDto> dtoList, CancellationToken ct = default);

    Task<StakeHolder> UpdateStakeHolderAsync(StakeHolderDto dto, long id, CancellationToken ct = default);

    Task DeleteStakeHolderByIdAsync(long id, CancellationToken ct = default);

    Task SetStakeHolderPriorityAsync(StakeHolderDto stakeHolderDto, CancellationToken ct = default);

    Task<List<StakeHolderDto>> FindAllStakeHoldersByPlanIdWithPriorityAsync(string body, CancellationToken ct = default);

    Task<StakeHolder> UpdatePowerInterestAsync(StakeHolderDto dto, CancellationToken ct = default);

    Task<StakeHolder> DeletePowerInterestAsync(StakeHolderDto dto, CancellationToken ct = default);
}
