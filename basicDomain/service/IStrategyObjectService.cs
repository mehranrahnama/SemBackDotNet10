using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.Base.model.dto;
using System.Collections.Generic;

namespace SemBackDotNet10.basicDomain.service;

public interface IStrategyObjectService
{
    StrategyObject StrategyObjectCreate(StrategyObjectDto strategyObjectDto);
    StrategyObject StrategyObjectEdit(StrategyObjectDto strategyObjectDto);
    StrategyObjectDto MapToDto(StrategyObject strategyObject);
    SearchDto FindAll(string search, int page, int size, string orderBy, System.ComponentModel.ListSortDirection direction);
    SearchDto FindAllByLevelId(LoadStrategyObjectListDto body, int page, int size, string orderBy, System.ComponentModel.ListSortDirection direction);
    SearchDto FindAllByLevelIdAndPerspectId(string body, int page, int size, string orderBy, System.ComponentModel.ListSortDirection direction, long tenantId);
    List<Model.Dto.StrategyObjectTitleDto> FindAllByLevelIdAndPlanId(string body, long tenantId);
    StrategyObject FindByIdOrElseThrow(long id);
    StrategyObject FindByIdAndPlanIdOrElseThrow(long id, long planId);
    List<StrategyObject> InsertStrategyObjectAndActionPlanIfNotExist(long planId, long lastPlanId, ActionPlan actionPlan);
    List<StrategyObject> FindDeletedStrategyObjectByPerspectiveId(long perspectiveId);
    List<StrategyObject> FindStrategyObjectByPerspectiveId(long perspectiveId);
    long StrategyObjectCount(long planId);
    long CountActiveGoalsByPlanIdLevelId(long planId, long levelId);
    List<StrategyObject> GetParentStrategyObj(long levelId);
    void DeleteById(long id);
    StrategyObject CreateStrategyObjectForLevelId(StrategyObject source, PlaningLevel planingLevel);
    StrategyObject? GetStrategyObjectViaDetailsForPlanningLevels(long planId, string title, PlaningLevel planningLevel);
    List<long> FindStrategyObjectByTitleAndLevelId(long strategyObjectId, long id);
    StrategyObject FindStrategyObjectWithChangedPlanningLevel(StrategyObject strategyObject, PlaningLevel level);
    StrategyObject FindOrCreateStrategyObject(StrategyObject source, PlaningLevel targetLevel, PlanPeriod targetPlanPeriod);
}
