using SemBackDotNet10.riskDomain.model.dto;
using SemBackDotNet10.riskDomain.model.entity;
using SemBackDotNet10.riskDomain.repository;

namespace SemBackDotNet10.riskDomain.service.interf;

public interface IRiskActionService
{
    IRiskActionRepository GetRepository();

    Task<RiskAction> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task<RiskAction> CreateRiskActionAsync(RiskActionDto riskDto, CancellationToken ct = default);

    Task DeleteByRiskIdAsync(long? riskId, CancellationToken ct = default);

    Task DeleteRiskActionByIdAsync(long? riskActionId, CancellationToken ct = default);

    Task EditRiskActionByIdAsync(RiskActionDto riskActionDto, CancellationToken ct = default);

    Task EditRiskActionListAsync(RiskDto riskDto, CancellationToken ct = default);
}
