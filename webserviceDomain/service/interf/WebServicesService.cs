using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;
using SemBackDotNet10.webserviceDomain.domain.dto;

namespace SemBackDotNet10.webserviceDomain.service.interf;

public interface IWebServicesService
{
    Task<SearchDto> FindAllStrategyServicesAsync(int page, int size, string? orderBy, SortDirection? direction, CancellationToken ct = default);

    Task<List<ActionPlanWebServiceDto>> FindAllActionPlansAsync(CancellationToken ct = default);

    Task<double> GetObjectFunctionalityAsync(CancellationToken ct = default);

    Task<double> GetActionPCNScoreAsync(CancellationToken ct = default);
}
