using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
namespace SemBackDotNet10.basicDomain.service;

public interface IPlanPeriodService
{
    SearchDto FindAll(string search, int page, int size, string orderBy, System.ComponentModel.ListSortDirection direction);
    PlanPeriodDto MapToDto(PlanPeriod planPeriod);
    PlanPeriod Create(PlanPeriodDto planPeriodDto);
    PlanPeriod CreatePlanPeriodByPlanId(long planId);
    PlanPeriod Edit(PlanPeriodDto planPeriodDto);
    PlanPeriod FindByIdOrElseThrow(int id);
    PlanPeriod FindByPlanId(long planId);
    PlanPeriod FindPlanPeriodByPlanId(long planId);
    PlanPeriod FindByPlanIdOrCreate(long planId);
    PlanPeriod FindActivePlanPeriod();
}
