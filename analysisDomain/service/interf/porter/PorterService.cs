using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using SemBackDotNet10.analysisDomain.repository.porter;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.interf.porter;

public interface IPorterService
{
    Task<Porter> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    IPorterRepository GetRepository();

    Task<PorterDto> CreateNewPorterAsync(PorterDto porterDto, CancellationToken ct = default);

    Task UpdatePorterAsync(PorterDto porterDto, CancellationToken ct = default);

    Task DeletePorterByIdAsync(long? porterId, CancellationToken ct = default);

    Task<SearchDto> GetAllPorterPageByPorterGroupAsync(int page, int size, string? orderBy, SortDirection? direction, string? body, CancellationToken ct = default);

    Task<List<PorterDto>> GetAllPorterByPorterGroupAsync(string? body, CancellationToken ct = default);
}
