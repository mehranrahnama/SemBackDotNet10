using SemBackDotNet10.dashboardDomain.model.dto;

namespace SemBackDotNet10.dashboardDomain.repository;

public interface IDashboardDtoRepository
{
    Task<List<DashboardDto>> FindAllMeasureActPlanByDashboardUserIdAsync(long planId, long userId);
}

public class DashboardDtoRepository : IDashboardDtoRepository
{
    private readonly IDashboardRepository _repository;

    public DashboardDtoRepository(IDashboardRepository repository) => _repository = repository;

    public async Task<List<DashboardDto>> FindAllMeasureActPlanByDashboardUserIdAsync(long planId, long userId)
    {
        var rows = await _repository.ExecAsync("DashboardGetAllMeasureActPlanDataByUserIdPlanId", planId, userId);
        return DbRowMapper.MapAll<DashboardDto>(rows);
    }
}
