using Common.exception;
using Common.utils;
using SemBackDotNet10.analysisDomain.model.dto.resource;
using SemBackDotNet10.analysisDomain.model.entity.resource;
using SemBackDotNet10.analysisDomain.repository.resource;
using SemBackDotNet10.analysisDomain.service.interf.resource;
using SemBackDotNet10.Base.model.dto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.Common.exception;
using Microsoft.Extensions.Configuration;
using SemBackDotNet10.service.impl;


namespace SemBackDotNet10.analysisDomain.service.impl.resource;

public class ResourceServiceImpl : IResourceService
{
    private readonly IConfiguration _configuration;
    private readonly IResourceRepository _resourceRepository;
    private readonly global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository _planPeriodService;
    private readonly PlaningLevelServiceImpl _planingLevelService;
    private readonly IResourceCompetencyRuleService _resourceCompetencyRuleService;

    public ResourceServiceImpl(
        IConfiguration configuration,
        IResourceRepository resourceRepository,
        global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository planPeriodService,
        PlaningLevelServiceImpl planingLevelService,
        IResourceCompetencyRuleService resourceCompetencyRuleService)
    {
        _configuration = configuration;
        _resourceRepository = resourceRepository;
        _planPeriodService = planPeriodService;
        _planingLevelService = planingLevelService;
        _resourceCompetencyRuleService = resourceCompetencyRuleService;
    }

    public async Task<Resource> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.Resource.idIsNull");
        return await _resourceRepository.FindByIdAsync(id.Value, ct)
            ?? throw new BusinessException("error.Resource.idIsNotValid");
    }

    public async Task<ResourceDto> CreateNewResourceAsync(ResourceDto resourceDto, CancellationToken ct = default)
    {
        if (resourceDto == null)
            throw new BusinessException("error.ResourceDto.isNull");
        var entity = await MapToEntityAsync(resourceDto, ct);
        var result = await _resourceRepository.AddAsync(entity, ct);
        return MapToDto(result);
    }

    public async Task<SearchDto> GetAllResourcePageByLevelIdAsync(int page, int size, string? orderBy, SortDirection? direction, string body, CancellationToken ct = default)
    {
        var json = AnalysisJsonHelper.Parse(body);
        var filter = AnalysisJsonHelper.OptString(json, "filter");
        var levelId = AnalysisJsonHelper.GetLong(json, "levelId");
        var planId = AnalysisJsonHelper.GetLong(json, "planId");
        var pageable = PageUtils.GetInstance(page, size, orderBy, direction);
        var result = await _resourceRepository.FindAllPageAsync(levelId, planId, filter, pageable, ct);

        return new SearchDto.Builder()
            .PageSize(size).CurrentPage(page).TotalPages(result.TotalPages)
            .TotalElements(result.TotalElements)
            .Content(result.Items)
            .Build();
    }

    public async Task<List<ResourceDto>> GetAllResourceByLevelIdAsync(string body, CancellationToken ct = default)
    {
        var json = AnalysisJsonHelper.Parse(body);
        var filter = AnalysisJsonHelper.OptString(json, "filter");
        var levelId = AnalysisJsonHelper.GetLong(json, "levelId");
        var planId = AnalysisJsonHelper.GetLong(json, "planId");
        return await _resourceRepository.FindAllAsync(levelId, planId, filter, ct);
    }

    public async Task UpdateResourceAsync(ResourceDto resourceDto, CancellationToken ct = default)
    {
        var resource = await FindByIdOrElseThrowAsync(resourceDto.Id, ct);

        resource.Title = resourceDto.Title ?? string.Empty;
        resource.ExistenceStatus = resourceDto.ExistenceStatus ?? false;
        resource.IsResource = resourceDto.Resource;
        resource.Power = resourceDto.Power ?? 0;
        resource.Importance = resourceDto.Importance ?? 0;
        await _resourceRepository.UpdateAsync(resource, ct);
    }

    public async Task DeleteResourceByIdAsync(long? resourceId, CancellationToken ct = default)
    {
        var result = await FindByIdOrElseThrowAsync(resourceId, ct);
        await _resourceRepository.DeleteAsync(result, ct);
    }

    public async Task EditResourceCompetencyAsync(ResourceDto resourceDto, CancellationToken ct = default)
    {
        var resource = await FindByIdOrElseThrowAsync(resourceDto.Id, ct);

        resource.Valuable = resourceDto.Valuable;
        resource.Rare = resourceDto.Rare;
        resource.CostlyInitiate = resourceDto.CostlyInitiate;
        resource.Organize = resourceDto.Organize;
        await SetCompetencyResultAsync(resource, ct);
        await _resourceRepository.UpdateAsync(resource, ct);
    }

    public async Task<SearchDto> GetKeyResourceByPlanIdAsync(int page, int size, string? orderBy, SortDirection? direction, string body, CancellationToken ct = default)
    {
        var pageable = PageUtils.GetInstance(page, size, orderBy, direction);
        var json = AnalysisJsonHelper.Parse(body);
        var planId = AnalysisJsonHelper.GetLong(json, "planId");
        var result = await _resourceRepository.FindKeyResourceByPlanIdAsync(planId, pageable, ct);
        return new SearchDto.Builder()
            .PageSize(size).CurrentPage(page).TotalPages(result.TotalPages)
            .TotalElements(result.TotalElements)
            .Content(result.Items)
            .Build();
    }

    public async Task<List<ResourceDto>> GetKeyResourceByPlanIdListAsync(string body, CancellationToken ct = default)
    {
        var json = AnalysisJsonHelper.Parse(body);
        var planId = AnalysisJsonHelper.GetLong(json, "planId");
        var tenantName = _configuration["sem.default.tenant.name"]
            ?? _configuration["sem:default:tenant:name"]
            ?? _configuration["Security:DefaultTenantName"]
            ?? throw new InvalidOperationException("Default tenant name is not configured.");
        if (tenantName != "MSCHOLDING")
            return await _resourceRepository.GetKeyResourceByPlanIdListIrisaAsync(planId, ct);
        return await _resourceRepository.GetKeyResourceByPlanIdListAsync(planId, ct);
    }

    private static ResourceDto MapToDto(Resource resource) => new()
    {
        Id = resource.Id,
        Title = resource.Title,
        Importance = resource.Importance,
        Resource = resource.IsResource,
        Power = resource.Power,
        PlanId = resource.PlanPeriod?.PlanId ?? resource.PlanId,
        PlaningLevelDto = new PlaningLevelDto
        {
            Id = resource.PlaningLevel?.Id ?? resource.PlaningLevelId ?? 0,
            DesLevel = resource.PlaningLevel?.DesLevel
        },
        ExistenceStatus = resource.ExistenceStatus
    };

    private async Task<Resource> MapToEntityAsync(ResourceDto resourceDto, CancellationToken ct)
    {
        var entity = new Resource
        {
            ExistenceStatus = resourceDto.ExistenceStatus ?? false,
            IsResource = resourceDto.Resource,
            Importance = resourceDto.Importance ?? 0,
            Power = resourceDto.Power ?? 0,
            Title = resourceDto.Title ?? string.Empty
        };
        if (resourceDto.PlanId != null)
        {
            var planPeriod = await _planPeriodService.FindPlanPeriodByPlanIdAsync(resourceDto.PlanId.Value, ct);
            entity.PlanPeriod = planPeriod;
            entity.PlanId = planPeriod.PlanId;
        }
        if (resourceDto.PlaningLevelDto != null)
        {
            var level = await _planingLevelService.FindByIdOrElseThrowAsync(resourceDto.PlaningLevelDto.Id, ct);
            entity.PlaningLevel = level;
            entity.PlaningLevelId = level.Id;
        }
        return entity;
    }

    private async Task SetCompetencyResultAsync(Resource resource, CancellationToken ct)
    {
        var planId = resource.PlanId ?? throw new BusinessException("error.Resource.planIdIsNull");
        var rule = await _resourceCompetencyRuleService.FindByPlanIdAsync(planId, ct);
        if (resource.Organize == true)
            resource.CompetencyResult = rule.FinalResult;
        else if (resource.CostlyInitiate == true)
            resource.CompetencyResult = rule.ResultO;
        else if (resource.Rare == true)
            resource.CompetencyResult = rule.ResultI;
        else if (resource.Valuable == true)
            resource.CompetencyResult = rule.ResultR;
        else
            resource.CompetencyResult = rule.ResultV;
    }
}
