using SemBackDotNet10.Common.exception;
using global::Common.logging;
using global::Common.messages;
using SemBackDotNet10.dashboardDomain.repository;

namespace SemBackDotNet10.dashboardDomain.domain;

public class DashboardDomainService
{
    private readonly IDashboardRepository _repository;
    private readonly SemLogger _logger;

    public DashboardDomainService(IDashboardRepository repository, SemLogger logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task RemoveDashboardChartByMeasureId(long measureId)
    {
        try
        {
            await _repository.DeleteByMeasureIdAsync(measureId);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to delete dashboard charts for measure {measureId}: {ex}");
            throw new BusinessException(BusinessMessages.DASHBOARD_DELETE_FAILED);
        }
    }

    public async Task RemoveDashboardChartByCatalogId(long catalogId)
    {
        try
        {
            await _repository.DeleteByMeasureCatalogIdAsync(catalogId);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to delete dashboard charts for catalog {catalogId}: {ex}");
            throw new BusinessException(BusinessMessages.DASHBOARD_DELETE_FAILED);
        }
    }
}
