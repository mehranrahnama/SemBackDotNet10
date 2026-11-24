using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.measureDomain.model.entity;
using SemBackDotNet10.riskDomain.domain;
using SemBackDotNet10.riskDomain.mapper;
using SemBackDotNet10.riskDomain.model.dto;
using SemBackDotNet10.riskDomain.model.entity;
using SemBackDotNet10.riskDomain.repository;
using SemBackDotNet10.riskDomain.service.interf;

namespace SemBackDotNet10.riskDomain.service.impl;

public class RiskMeasureServiceImpl : IRiskMeasureService
{
    private readonly IRiskMeasureRepository _riskMeasureRepository;
    private readonly IRiskMeasureMapper _riskMeasureMapper;
    private readonly IRiskDomainService _riskDomainService;
    private readonly SemDbContext _context;

    public RiskMeasureServiceImpl(
        IRiskMeasureRepository riskMeasureRepository,
        IRiskMeasureMapper riskMeasureMapper,
        IRiskDomainService riskDomainService,
        SemDbContext context)
    {
        _riskMeasureRepository = riskMeasureRepository;
        _riskMeasureMapper = riskMeasureMapper;
        _riskDomainService = riskDomainService;
        _context = context;
    }

    public IRiskMeasureRepository GetRepository() => _riskMeasureRepository;

    public async Task<RiskMeasure> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id == null)
            throw new BusinessException("error.RiskMeasure.idIsNull");
        return await GetRepository().FindByIdAsync(id.Value, ct)
               ?? throw new BusinessException("", id.Value.ToString());
    }

    public async Task<RiskMeasure> CreateRiskMeasureAsync(RiskMeasureDto riskMeasureDto, CancellationToken ct = default)
    {
        var entity = await _riskMeasureMapper.ToEntityAsync(riskMeasureDto, ct);
        return await GetRepository().AddAsync(entity, ct);
    }

    public async Task DeleteByRiskIdAsync(long? riskId, CancellationToken ct = default)
    {
        var items = await GetRepository().FindByRiskIdAsync(riskId, ct);
        await GetRepository().DeleteRangeAsync(items, ct);
    }

    public async Task DeleteRiskMeasureByIdAsync(long? riskMeasureId, CancellationToken ct = default)
    {
        var riskMeasure = await FindByIdOrElseThrowAsync(riskMeasureId, ct);
        await GetRepository().DeleteAsync(riskMeasure, ct);
    }

    public async Task EditRiskMeasureByIdAsync(RiskMeasureDto riskMeasureDto, CancellationToken ct = default)
    {
        if (riskMeasureDto.Id != null)
        {
            var riskMeasure = await FindByIdOrElseThrowAsync(riskMeasureDto.Id, ct);

            riskMeasure.RiskMeasureTitle = riskMeasureDto.MeasureCatalogDto == null
                ? riskMeasureDto.RiskMeasureTitle
                : null;

            if (riskMeasureDto.RiskMeasureTitle == null)
            {
                var catalogId = riskMeasureDto.MeasureCatalogDto!.Id;
                if (catalogId == null)
                    throw new BusinessException("error.MeasureCatalog.idIsNull");
                var catalog = await _context.Set<MeasureCatalog>().FindAsync(new object[] { catalogId.Value }, ct)
                              ?? throw new BusinessException("error.MeasureCatalog.idIsNotValid");
                riskMeasure.MeasureCatalog = catalog;
                riskMeasure.MeasureCatalogId = catalogId;
            }
            else
            {
                riskMeasure.MeasureCatalog = null;
                riskMeasure.MeasureCatalogId = null;
            }

            await GetRepository().UpdateAsync(riskMeasure, ct);
        }
        else
        {
            await CreateRiskMeasureAsync(riskMeasureDto, ct);
        }
    }

    public async Task EditRiskMeasureListAsync(RiskDto riskDto, CancellationToken ct = default)
    {
        var risk = await _riskDomainService.FindByIdOrElseThrowAsync(riskDto.RiskId, ct);

        var existing = (await GetRepository().FindByRiskIdAsync(risk.Id, ct))
            .ToDictionary(m => m.Id);

        foreach (var dto in riskDto.RiskMeasureDtoList)
        {
            // create
            if (dto.Id == null)
            {
                await CreateRiskMeasureAsync(dto, ct);
                continue;
            }

            // update
            if (existing.Remove(dto.Id.Value))
                await EditRiskMeasureByIdAsync(dto, ct);
        }

        // delete
        foreach (var deleted in existing.Keys.ToList())
            await DeleteRiskMeasureByIdAsync(deleted, ct);
    }
}
