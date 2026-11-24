using Common.exception;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.riskDomain.domain;
using SemBackDotNet10.riskDomain.model.dto;
using SemBackDotNet10.riskDomain.model.entity;
using SemBackDotNet10.riskDomain.repository;
using SemBackDotNet10.riskDomain.service.interf;

namespace SemBackDotNet10.riskDomain.service.impl;

public class RiskActionServiceImpl : IRiskActionService
{
    private readonly IRiskActionRepository _riskActionRepository;
    private readonly IRiskDomainService _riskDomainService;
    private readonly SemDbContext _context;

    public RiskActionServiceImpl(
        IRiskActionRepository riskActionRepository,
        IRiskDomainService riskDomainService,
        SemDbContext context)
    {
        _riskActionRepository = riskActionRepository;
        _riskDomainService = riskDomainService;
        _context = context;
    }

    public IRiskActionRepository GetRepository() => _riskActionRepository;

    public async Task<RiskAction> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.RiskActino.idIsNull");
        return await GetRepository().FindByIdAsync(id.Value, ct)
               ?? throw new BusinessException("", id.Value.ToString());
    }

    public async Task<RiskAction> CreateRiskActionAsync(RiskActionDto riskActionDto, CancellationToken ct = default)
    {
        var entity = await MapToEntityAsync(riskActionDto, ct);
        return await GetRepository().AddAsync(entity, ct);
    }

    public async Task DeleteByRiskIdAsync(long? riskId, CancellationToken ct = default)
    {
        var items = await GetRepository().FindByRiskIdAsync(riskId, ct);
        await GetRepository().DeleteRangeAsync(items, ct);
    }

    public async Task DeleteRiskActionByIdAsync(long? riskActionId, CancellationToken ct = default)
    {
        var riskAction = await FindByIdOrElseThrowAsync(riskActionId, ct);
        await GetRepository().DeleteAsync(riskAction, ct);
    }

    public async Task EditRiskActionByIdAsync(RiskActionDto riskActionDto, CancellationToken ct = default)
    {
        if (riskActionDto.Id == null)
            return;

        var riskAction = await FindByIdOrElseThrowAsync(riskActionDto.Id, ct);

        riskAction.RiskActionTitle = riskActionDto.ActionPlanCatalogDto == null
            ? riskActionDto.RiskActionTitle
            : null;

        if (riskActionDto.RiskActionTitle == null)
        {
            var catalog = await FindActionPlanCatalogOrElseThrowAsync(riskActionDto.ActionPlanCatalogDto!.Id, ct);
            riskAction.ActionPlanCatalog = catalog;
            riskAction.ActionPlanCatalogId = catalog.Id;
        }
        else
        {
            riskAction.ActionPlanCatalog = null;
            riskAction.ActionPlanCatalogId = null;
        }

        riskAction.Description = riskActionDto.Description;
        riskAction.ProgressPercent = riskActionDto.ProgressPercent;
        await GetRepository().UpdateAsync(riskAction, ct);
    }

    private async Task<RiskAction> MapToEntityAsync(RiskActionDto riskActionDto, CancellationToken ct)
    {
        var risk = await _riskDomainService.FindByIdOrElseThrowAsync(riskActionDto.RiskId, ct);
        var riskAction = new RiskAction
        {
            Risk = risk,
            RiskId = risk.Id,
            RiskActionType = riskActionDto.RiskActionType,
            RiskActionTitle = riskActionDto.RiskActionTitle,
            Description = riskActionDto.Description,
            ProgressPercent = riskActionDto.ProgressPercent
        };

        if (riskActionDto.ActionPlanCatalogDto != null)
        {
            var catalog = await FindActionPlanCatalogOrElseThrowAsync(riskActionDto.ActionPlanCatalogDto.Id, ct);
            riskAction.ActionPlanCatalog = catalog;
            riskAction.ActionPlanCatalogId = catalog.Id;
        }

        return riskAction;
    }

    private async Task<ActionPlanCatalog> FindActionPlanCatalogOrElseThrowAsync(long? id, CancellationToken ct)
    {
        if (id == null)
            throw new BusinessException("error.ActionPlanCatalog.idIsNull");
        return await _context.Set<ActionPlanCatalog>().FindAsync(new object[] { id.Value }, ct)
               ?? throw new BusinessException("error.ActionPlanCatalog.idIsNotValid");
    }

    public async Task EditRiskActionListAsync(RiskDto riskDto, CancellationToken ct = default)
    {
        var risk = await _riskDomainService.FindByIdOrElseThrowAsync(riskDto.RiskId, ct);

        var existing = (await GetRepository().FindByRiskIdAsync(risk.Id, ct))
            .ToDictionary(a => a.Id);

        foreach (var dto in riskDto.RiskActionDtoList)
        {
            // create
            if (dto.Id == null)
            {
                await CreateRiskActionAsync(dto, ct);
                continue;
            }

            // update
            if (existing.Remove(dto.Id.Value))
                await EditRiskActionByIdAsync(dto, ct);
        }

        // delete
        foreach (var deleted in existing.Keys.ToList())
            await DeleteRiskActionByIdAsync(deleted, ct);
    }
}
