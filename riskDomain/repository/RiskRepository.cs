using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.riskDomain.model.dto;
using SemBackDotNet10.riskDomain.model.entity;

namespace SemBackDotNet10.riskDomain.repository;

public interface IRiskRepository : IBaseRepository<Risk>
{
    Task<List<RiskDto>> FindByPlanIdAndLevelIdAsync(long? planId, long? levelId, string? filter, CancellationToken ct = default);
}

public class RiskRepository : BaseRepository<Risk>, IRiskRepository
{
    public RiskRepository(SemDbContext context) : base(context) { }

    public Task<List<RiskDto>> FindByPlanIdAndLevelIdAsync(long? planId, long? levelId, string? filter, CancellationToken ct = default) =>
        Query
            .Where(r => planId != null && r.StrategyObject!.Perspective!.PlanId == planId)
            .Where(r => levelId != null && r.StrategyObject!.PlaningLevelId == levelId)
            .Where(r => filter == null || r.StrategyObject!.DesStrategyObjectF!.Contains(filter))
            .Select(r => new RiskDto
            {
                RiskId = r.Id,
                DesRisk = r.DesRisk,
                Cause = r.Cause,
                Effect = r.Effect,
                Severity = r.Severity,
                Probability = r.Probability,
                StrategyObjectDto = new RiskStrategyObjectDto
                {
                    Id = r.StrategyObject!.Id,
                    Title = r.StrategyObject.DesStrategyObjectF,
                    LevelId = r.StrategyObject.PlaningLevelId
                }
            })
            .ToListAsync(ct);
}
