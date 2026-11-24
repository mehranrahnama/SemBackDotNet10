using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.riskDomain.model.dto;
using SemBackDotNet10.riskDomain.repository;

namespace SemBackDotNet10.riskDomain.service.interf;

public interface IRiskService
{
    IRiskRepository GetRepository();

    Task CreateRiskAsync(RiskDto riskDto, CancellationToken ct = default);

    Task<SearchDto> GetRiskByPlanIdAndLevelIdAsync(string body, int page, int size, string? filter, string? orderBy, SortDirection? direction, CancellationToken ct = default);

    Task EditRiskAsync(RiskDto riskDto, CancellationToken ct = default);

    Task DeleteRiskAsync(long? riskId, CancellationToken ct = default);
}
