using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.measureDomain.model.dto;
using SemBackDotNet10.riskDomain.model.dto;
using SemBackDotNet10.riskDomain.model.entity;

namespace SemBackDotNet10.riskDomain.repository;

public interface IRiskMeasureRepository : IBaseRepository<RiskMeasure>
{
    Task<List<RiskMeasureDto>> FindByRiskIdInAsync(List<long> riskIds, CancellationToken ct = default);

    Task<List<RiskMeasure>> FindByRiskIdAsync(long? riskId, CancellationToken ct = default);
}

public class RiskMeasureRepository : BaseRepository<RiskMeasure>, IRiskMeasureRepository
{
    public RiskMeasureRepository(SemDbContext context) : base(context) { }

    public Task<List<RiskMeasureDto>> FindByRiskIdInAsync(List<long> riskIds, CancellationToken ct = default) =>
        Query
            .Where(rm => rm.RiskId != null && riskIds.Contains(rm.RiskId.Value))
            .Select(rm => new RiskMeasureDto
            {
                Id = rm.Id,
                RiskId = rm.RiskId,
                RiskMeasureTitle = rm.RiskMeasureTitle,
                MeasureCatalogDto = rm.MeasureCatalogId != null
                    ? new MeasureCatalogDto
                    {
                        Id = rm.MeasureCatalogId,
                        MeasureName = rm.MeasureCatalog!.MeasureRepository.MeasureName
                    }
                    : null
            })
            .ToListAsync(ct);

    public Task<List<RiskMeasure>> FindByRiskIdAsync(long? riskId, CancellationToken ct = default) =>
        Query.Where(rm => rm.RiskId == riskId).ToListAsync(ct);
}
