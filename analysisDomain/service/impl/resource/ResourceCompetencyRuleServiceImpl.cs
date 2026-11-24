using SemBackDotNet10.analysisDomain.model.dto.resource;
using SemBackDotNet10.analysisDomain.model.entity.resource;
using SemBackDotNet10.analysisDomain.repository.resource;
using SemBackDotNet10.analysisDomain.service.interf.resource;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.analysisDomain.service.impl.resource;

public class ResourceCompetencyRuleServiceImpl : IResourceCompetencyRuleService
{
    private readonly IResourceCompetencyRuleRepository _resourceCompetencyRuleRepository;
    private readonly global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository _planPeriodService;

    public ResourceCompetencyRuleServiceImpl(
        IResourceCompetencyRuleRepository resourceCompetencyRuleRepository,
        global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository planPeriodService)
    {
        _resourceCompetencyRuleRepository = resourceCompetencyRuleRepository;
        _planPeriodService = planPeriodService;
    }

    public async Task<ResourceCompetencyRuleDto> CreateNewResourceCompetencyRuleAsync(ResourceCompetencyRuleDto resourceCompetencyRuleDto, CancellationToken ct = default)
    {
        if (resourceCompetencyRuleDto == null)
            throw new BusinessException("error.resourceCompetencyRuleDto.isNull");
        if (resourceCompetencyRuleDto.PlanId == null)
            throw new BusinessException("error.PlanPeriod.planIdIsNull");

        var existed = await _resourceCompetencyRuleRepository.FindByPlanIdAsync(resourceCompetencyRuleDto.PlanId.Value, ct);
        if (existed != null)
            throw new BusinessException("برای سال جاری مقادیر VRIO تعریف شده است.");
        var entity = await MapToEntityAsync(resourceCompetencyRuleDto, ct);
        var result = await _resourceCompetencyRuleRepository.AddAsync(entity, ct);
        return MapToDto(result);
    }

    public async Task<ResourceCompetencyRule> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.ResourceCompetencyRule.idIsNull");
        return await _resourceCompetencyRuleRepository.FindByIdAsync(id.Value, ct)
            ?? throw new BusinessException("error.ResourceCompetencyRule.idIsNotValid");
    }

    public Task<ResourceCompetencyRuleDto?> GetResourceCompetencyRuleByPlanIdAsync(long planId, CancellationToken ct = default) =>
        _resourceCompetencyRuleRepository.GetResourceCompetencyRuleByPlanIdAsync(planId, ct);

    public async Task<ResourceCompetencyRule> FindByPlanIdAsync(long planId, CancellationToken ct = default)
    {
        var result = await _resourceCompetencyRuleRepository.FindByPlanIdAsync(planId, ct);
        if (result == null)
            throw new BusinessException("قانون تعیین شایستگی برای سال جاری مشخص نشده است.");
        return result;
    }

    private static ResourceCompetencyRuleDto MapToDto(ResourceCompetencyRule rule) => new()
    {
        Id = rule.Id,
        ValueV = rule.ValueV,
        ResultV = rule.ResultV,
        ValueR = rule.ValueR,
        ResultR = rule.ResultR,
        ValueI = rule.ValueI,
        ResultI = rule.ResultI,
        ValueO = rule.ValueO,
        ResultO = rule.ResultO,
        PlanId = rule.PlanPeriod?.PlanId ?? rule.PlanId,
        FinalResult = rule.FinalResult
    };

    private async Task<ResourceCompetencyRule> MapToEntityAsync(ResourceCompetencyRuleDto dto, CancellationToken ct)
    {
        var planPeriod = await _planPeriodService.FindPlanPeriodByPlanIdAsync(dto.PlanId!.Value, ct);
        return new ResourceCompetencyRule
        {
            ValueV = dto.ValueV ?? string.Empty,
            ResultV = dto.ResultV ?? string.Empty,
            ValueR = dto.ValueR ?? string.Empty,
            ResultR = dto.ResultR ?? string.Empty,
            ValueI = dto.ValueI ?? string.Empty,
            ResultI = dto.ResultI ?? string.Empty,
            ValueO = dto.ValueO ?? string.Empty,
            ResultO = dto.ResultO ?? string.Empty,
            PlanPeriod = planPeriod,
            PlanId = planPeriod.PlanId,
            FinalResult = dto.FinalResult ?? string.Empty
        };
    }
}
