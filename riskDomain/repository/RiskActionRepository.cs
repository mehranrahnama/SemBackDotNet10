using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.riskDomain.model.dto;
using SemBackDotNet10.riskDomain.model.entity;

namespace SemBackDotNet10.riskDomain.repository;

public interface IRiskActionRepository : IBaseRepository<RiskAction>
{
    Task<List<RiskActionDto>> FindByRiskIdInAsync(List<long> riskIds, CancellationToken ct = default);

    Task<List<RiskAction>> FindByRiskIdAsync(long? riskId, CancellationToken ct = default);
}

public class RiskActionRepository : BaseRepository<RiskAction>, IRiskActionRepository
{
    public RiskActionRepository(SemDbContext context) : base(context) { }

    public Task<List<RiskActionDto>> FindByRiskIdInAsync(List<long> riskIds, CancellationToken ct = default) =>
        Query
            .Where(ra => ra.RiskId != null && riskIds.Contains(ra.RiskId.Value))
            .Select(ra => new RiskActionDto
            {
                Id = ra.Id,
                RiskActionType = ra.RiskActionType,
                RiskActionTitle = ra.RiskActionTitle,
                ProgressPercent = ra.ProgressPercent,
                Description = ra.Description,
                RiskId = ra.RiskId,
                ActionPlanCatalogDto = ra.ActionPlanCatalogId != null
                    ? new ActionPlanCatalogDto
                    {
                        Id = ra.ActionPlanCatalogId,
                        Description = ra.ActionPlanCatalog!.Description
                    }
                    : null
            })
            .ToListAsync(ct);

    public Task<List<RiskAction>> FindByRiskIdAsync(long? riskId, CancellationToken ct = default) =>
        Query.Where(ra => ra.RiskId == riskId).ToListAsync(ct);
}
