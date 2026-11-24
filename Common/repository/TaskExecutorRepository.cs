using Common.model.entity;
namespace Common.repository;
public interface IReportExecutionQueries
{
    // Must implement the exact Java LIKE and null predicates and reject non-unique results.
    ReportExecutedData? FindReportsStatusByPlanIdAndLevelId(long? planId,long? levelId,long? month,string storedProcedureName);
    // Transactional parameterized dbo.Report_ActionProgressPercent_NotHosco_AllMonths_Online(PlanId,LevelId).
    void ExecuteActionProgressPercentNotHoscoAllMonths(int planId,int levelId);
}
public sealed class TaskExecutorRepository(IPersistenceSession<ReportExecutedData> session,IReportExecutionQueries queries):EntityRepository<ReportExecutedData,long?>(session,r=>r.Id)
{
    public ReportExecutedData? FindReportsStatusByPlanIdAndLevelId(long? planId,long? levelId,long? month,string storedProcedureName)=>queries.FindReportsStatusByPlanIdAndLevelId(planId,levelId,month,storedProcedureName);
    public void ExecuteActionProgressPercentNotHoscoAllMonths(int planId,int levelId)=>queries.ExecuteActionProgressPercentNotHoscoAllMonths(planId,levelId);
}
