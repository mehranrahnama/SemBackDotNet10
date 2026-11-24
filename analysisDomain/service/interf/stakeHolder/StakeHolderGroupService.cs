using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.stakeHolder;
using SemBackDotNet10.analysisDomain.model.entity.stakeHolder;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.interf.stakeHolder;

public interface IStakeHolderGroupService
{
    Task<StakeHolderGroup> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task<SearchDto> GetAllStakeHolderGroupsAsync(string? filter, string? body, int page, int size, string? orderBy, SortDirection? direction, CancellationToken ct = default);

    Task CreateStakeHolderGroupsAsync(List<StakeHolderGroupDto> dtoList, CancellationToken ct = default);

    Task DeleteStakeHolderGroupByIdAsync(long id, CancellationToken ct = default);

    Task<StakeHolderGroup> UpdateStakeHolderGroupByIdAsync(long id, StakeHolderGroupDto dto, CancellationToken ct = default);

    Task<List<StakeHolderGroupDto>> GetAllStakeHolderGroupAsync(long planId, CancellationToken ct = default);
}
