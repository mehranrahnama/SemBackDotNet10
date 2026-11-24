using SemBackDotNet10.analysisDomain.model.dto.resource;
using SemBackDotNet10.analysisDomain.model.entity.resource;

namespace SemBackDotNet10.analysisDomain.service.interf.resource;

public interface IResourceCompetencyRuleService
{
    Task<ResourceCompetencyRuleDto> CreateNewResourceCompetencyRuleAsync(ResourceCompetencyRuleDto resourceCompetencyRuleDto, CancellationToken ct = default);

    Task<ResourceCompetencyRule> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task<ResourceCompetencyRuleDto?> GetResourceCompetencyRuleByPlanIdAsync(long planId, CancellationToken ct = default);

    Task<ResourceCompetencyRule> FindByPlanIdAsync(long planId, CancellationToken ct = default);
}
