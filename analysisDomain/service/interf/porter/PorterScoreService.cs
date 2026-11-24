using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.interf.porter;

public interface IPorterScoreService
{
    Task<PorterScore> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task CreateNewPorterScoreAsync(List<PorterScoreDto> porterScoreDto, CancellationToken ct = default);

    Task DeletePorterScoreAsync(long? porterScoreId, CancellationToken ct = default);

    Task EditPorterScoreAsync(List<PorterScoreDto> porterScoreDto, CancellationToken ct = default);

    Task EditScoreAsync(List<PorterScoreDto> porterScoreDto, CancellationToken ct = default);

    Task<SearchDto> GetAllByPorterGroupIdAsync(long? porterGroupId, int page, int size, string? orderBy, SortDirection? direction, CancellationToken ct = default);

    Task<List<PorterScoreDto>> GetAllByPorterGroupIdAsync(long? porterGroupId, CancellationToken ct = default);

    Task<List<PorterScoreDto>> GetKeyOpportunityByPorterGroupAsync(string? body, CancellationToken ct = default);

    Task<List<PorterScoreDto>> GetKeyThreatByPorterGroupAsync(string? body, CancellationToken ct = default);

    Task<List<PorterScoreDto>> GetAllByPorterGroupAndProductAndMarketAsync(string? body, CancellationToken ct = default);
}
