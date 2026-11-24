using SemBackDotNet10.riskDomain.model.dto;
using SemBackDotNet10.riskDomain.model.entity;
using SemBackDotNet10.riskDomain.repository;

namespace SemBackDotNet10.riskDomain.service.interf;

public interface IRiskMeasureService
{
    IRiskMeasureRepository GetRepository();

    Task<RiskMeasure> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task<RiskMeasure> CreateRiskMeasureAsync(RiskMeasureDto riskDto, CancellationToken ct = default);

    Task DeleteByRiskIdAsync(long? riskId, CancellationToken ct = default);

    Task DeleteRiskMeasureByIdAsync(long? riskMeasureId, CancellationToken ct = default);

    Task EditRiskMeasureByIdAsync(RiskMeasureDto riskMeasureDto, CancellationToken ct = default);

    Task EditRiskMeasureListAsync(RiskDto riskDto, CancellationToken ct = default);
}
