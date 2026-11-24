using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;
using SemBackDotNet10.dashboardDomain.model.entity;

namespace SemBackDotNet10.dashboardDomain.repository;

public interface IDashboardPCNRepository
{
    Task<List<ReportDashboardPayeshPCN>> FindByPlanIdAndMonthAsync(long planId, long month);

    Task<List<ReportDashboardPayeshPCN>> FindByPlanIdAndLevelIdAsync(long planId, long levelId);

    Task ExecuteReportDashboardPayeshPCNAllLevelsByMonthAsync(long planId, long month);

    Task ExecuteReportDashboardPayeshPCNAllMonthByLevelIdAsync(long planId, long levelId);

    Task ExecuteReportAllLevelScoreByActionHoscoOnlineAsync(long planId, long month, long fetchType);
}

public class DashboardPCNRepository : IDashboardPCNRepository
{
    private readonly SemDbContext _context;
    private readonly IDashboardRepository _repository;

    public DashboardPCNRepository(SemDbContext context, IDashboardRepository repository)
    {
        _context = context;
        _repository = repository;
    }

    // Normalize SQL integer widths to match the existing report model.
    private IQueryable<ReportDashboardPayeshPCN> Reports() =>
        _context.Set<ReportDashboardPayeshPCN>().FromSqlRaw("""
            SELECT CAST(planid AS bigint) AS planid, CAST(month AS bigint) AS month,
                   CAST(levelid AS bigint) AS levelid, currentdate, deslevel,
                   actionpayeshdcount, actionnotpayeshdcount, measurepayeshdcount, measurenotpayeshdcount,
                   actionachievedcount, actionnotachievedcount, measureachievedcount, measurenotachievedcount
            FROM dbo.tblreportdashboardpayeshpcn
            """).AsNoTracking();

    public Task<List<ReportDashboardPayeshPCN>> FindByPlanIdAndMonthAsync(long planId, long month) =>
        Reports().Where(r => r.PlanId == planId && r.Month == month).ToListAsync();

    public Task<List<ReportDashboardPayeshPCN>> FindByPlanIdAndLevelIdAsync(long planId, long levelId) =>
        Reports().Where(r => r.PlanId == planId && r.LevelId == levelId).ToListAsync();

    public Task ExecuteReportDashboardPayeshPCNAllLevelsByMonthAsync(long planId, long month) =>
        _repository.ExecNonQueryAsync("Report_Dashboard_Payesh_PCN_AllLevelsByMonth", planId, month);

    public Task ExecuteReportDashboardPayeshPCNAllMonthByLevelIdAsync(long planId, long levelId) =>
        _repository.ExecNonQueryAsync("Report_Dashboard_Payesh_PCN_AllMonth_ByLevelId", planId, levelId);

    public Task ExecuteReportAllLevelScoreByActionHoscoOnlineAsync(long planId, long month, long fetchType) =>
        _repository.ExecNonQueryAsync("Report_AllLevelScore_ByAction_Hosco_Online", planId, month, fetchType);
}
