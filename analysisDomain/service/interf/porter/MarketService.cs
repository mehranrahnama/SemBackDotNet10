using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.interf.porter;

public interface IMarketService
{
    Task<Market> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task<MarketDto> CreateMarketAsync(MarketDto marketDto, CancellationToken ct = default);

    Task<SearchDto> FindAllPageAsync(int page, int size, string? orderBy, SortDirection? direction, string? body, CancellationToken ct = default);

    Task<List<MarketDto>> FindAllMarketAsync(CancellationToken ct = default);

    Task<MarketDto> EditMarketAsync(MarketDto marketDto, CancellationToken ct = default);

    Task DeleteMarketAsync(long? id, CancellationToken ct = default);

    Task<Market> MapToEntityAsync(MarketDto marketDto, CancellationToken ct = default);

    MarketDto MapToDto(Market market);
}
