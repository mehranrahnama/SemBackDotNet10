using SemBackDotNet10.dashboardDomain.model.dto;

namespace SemBackDotNet10.dashboardDomain.repository;

public interface IStrategyMapEvalByMeasureHoscoRepository
{
    Task<List<StrategyMapEvalByMeasureHoscoDto>> ReportStrategyMapEvalByMeasureByLevelIdHoscoAsync(long planId, long levelId, long month);
}

public class StrategyMapEvalByMeasureHoscoRepository : IStrategyMapEvalByMeasureHoscoRepository
{
    private readonly IDashboardRepository _repository;

    public StrategyMapEvalByMeasureHoscoRepository(IDashboardRepository repository) => _repository = repository;

    public async Task<List<StrategyMapEvalByMeasureHoscoDto>> ReportStrategyMapEvalByMeasureByLevelIdHoscoAsync(
        long planId, long levelId, long month)
    {
        var rows = await _repository.ExecAsync("Report_StrategyMap_Eval_ByMeasure_ByLevelId_Hosco", planId, levelId, month);
        return DbRowMapper.MapAll<StrategyMapEvalByMeasureHoscoDto>(rows);
    }
}
