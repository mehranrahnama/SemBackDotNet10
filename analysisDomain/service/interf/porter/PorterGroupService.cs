using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.interf.porter;

public interface IPorterGroupService
{
    Task<PorterGroup> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task<PorterGroupDto> CreatePorterGroupAsync(PorterGroupDto porterGroupDto, CancellationToken ct = default);

    Task<SearchDto> FindAllPageAsync(int page, int size, string? orderBy, SortDirection? direction, string? body, CancellationToken ct = default);

    Task<List<PorterGroupDto>> FindAllPorterGroupsAsync(long? planId, CancellationToken ct = default);

    Task<PorterGroupDto> EditPorterGroupAsync(PorterGroupDto porterGroupDto, CancellationToken ct = default);

    Task DeletePorterGroupAsync(long? id, CancellationToken ct = default);

    Task<PorterGroup> MapToEntityAsync(PorterGroupDto porterGroupDto, CancellationToken ct = default);

    PorterGroupDto MapToDto(PorterGroup porterGroup);
}
