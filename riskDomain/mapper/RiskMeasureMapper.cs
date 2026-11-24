using SemBackDotNet10.measureDomain.model.entity;
using SemBackDotNet10.riskDomain.domain;
using SemBackDotNet10.riskDomain.model.dto;
using SemBackDotNet10.riskDomain.model.entity;

namespace SemBackDotNet10.riskDomain.mapper;

public interface IRiskMeasureMapper
{
    /// <summary>id, risk and measureCatalog are not copied from the dto; risk and measureCatalog are resolved afterwards.</summary>
    Task<RiskMeasure> ToEntityAsync(RiskMeasureDto dto, CancellationToken ct = default);
}

public class RiskMeasureMapper : IRiskMeasureMapper
{
    private readonly IRiskDomainService _riskDomainService;
    private readonly SemDbContext _context;

    public RiskMeasureMapper(IRiskDomainService riskDomainService, SemDbContext context)
    {
        _riskDomainService = riskDomainService;
        _context = context;
    }

    public async Task<RiskMeasure> ToEntityAsync(RiskMeasureDto dto, CancellationToken ct = default)
    {
        var entity = new RiskMeasure
        {
            RiskMeasureTitle = dto.RiskMeasureTitle
        };

        if (dto.RiskId != null)
        {
            var risk = await _riskDomainService.FindByIdOrElseThrowAsync(dto.RiskId, ct);
            entity.Risk = risk;
            entity.RiskId = risk.Id;
        }

        if (dto.MeasureCatalogDto?.Id != null)
        {
            var catalogId = dto.MeasureCatalogDto.Id.Value;
            var catalog = await _context.Set<MeasureCatalog>().FindAsync(new object[] { catalogId }, ct)
                          ?? throw new BusinessException("error.MeasureCatalog.idIsNotValid");
            entity.MeasureCatalog = catalog;
            entity.MeasureCatalogId = catalogId;
        }

        return entity;
    }
}
