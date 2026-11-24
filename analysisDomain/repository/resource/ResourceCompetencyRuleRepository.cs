using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.resource;
using SemBackDotNet10.analysisDomain.model.entity.resource;
using SemBackDotNet10.basicDomain.Repositories;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.analysisDomain.repository.resource;

public interface IResourceCompetencyRuleRepository : IBaseRepository<ResourceCompetencyRule>
{
    Task<ResourceCompetencyRule?> FindByPlanIdAsync(long planId, CancellationToken ct = default);

    Task<ResourceCompetencyRuleDto?> GetResourceCompetencyRuleByPlanIdAsync(long planId, CancellationToken ct = default);
}

public class ResourceCompetencyRuleRepository : BaseRepository<ResourceCompetencyRule>, IResourceCompetencyRuleRepository
{
    public ResourceCompetencyRuleRepository(SemDbContext context) : base(context) { }

    public Task<ResourceCompetencyRule?> FindByPlanIdAsync(long planId, CancellationToken ct = default) =>
        Query.FirstOrDefaultAsync(r => r.PlanId == planId, ct);

    public Task<ResourceCompetencyRuleDto?> GetResourceCompetencyRuleByPlanIdAsync(long planId, CancellationToken ct = default) =>
        Query.Where(r => r.PlanId == planId)
            .Select(r => new ResourceCompetencyRuleDto
            {
                Id = r.Id,
                ValueV = r.ValueV,
                ResultV = r.ResultV,
                ValueR = r.ValueR,
                ResultR = r.ResultR,
                ValueI = r.ValueI,
                ResultI = r.ResultI,
                ValueO = r.ValueO,
                ResultO = r.ResultO,
                FinalResult = r.FinalResult,
                PlanId = r.PlanId
            })
            .FirstOrDefaultAsync(ct);
}
