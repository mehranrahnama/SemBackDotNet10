using global::SemBackDotNet10.Models.Entities;
namespace SemBackDotNet10.basicDomain.service;

public interface IStrategyPillarService
{
    StrategyPillar FindStrategyPillarDataByPlanIdAndLevelId(long planId, long levelId);
    StrategyPillar FindStrategyPillarDataByPlanIdAndLevelIdForRoadMap(long planId, long levelId);
    StrategyPillar FindStrategyPillarDataByPlanId(long planId);
    StrategyPillarDto StrategyPillarCreate(StrategyPillarDto strategyPillarDto);
    StrategyPillar FindByIdOrElseThrow(long id);
    StrategyPillarDto MapToDto(StrategyPillar strategyPillar);
}
