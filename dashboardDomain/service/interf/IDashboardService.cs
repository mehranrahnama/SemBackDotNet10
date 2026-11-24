using SemBackDotNet10.dashboardDomain.model.dto;
using SemBackDotNet10.dashboardDomain.model.entity;
using SemBackDotNet10.measureDomain.model.dto;
using StrategyMapEvalByMeasureHoscoDto = SemBackDotNet10.dashboardDomain.model.dto.StrategyMapEvalByMeasureHoscoDto;

namespace SemBackDotNet10.dashboardDomain.service.interf;

public interface IDashboardService
{
    Task CreateOrDelete(DashboardTobeDto dashboardDto);

    Task CreateOrDeleteBatchDashboard(List<DashboardTobeDto> dashboardDtoList);

    /// <summary>پارامترها مثل جاوا به ترتیب (planId, userId) از کنترلر می‌آیند.</summary>
    Task<List<object>> FindAllMeasureActPlanByDashboardUserId(long planId, long userId);

    Task<List<DashboardActionPlanDto>> FindAllActionActPlanByDashboardUserId(long planId, long userId);

    Task<CountReportDTO> GetCountReport(long planId);

    Task<CountReportDTO> GetCountReportByPlanIdAndLevelId(string? body);

    Task<List<FetchDashboardPCNDto>> GetPayeshPCNReport(DashboardPCNDto dto);

    Task<List<FetchDashboardPCNDto>> GetPayeshPCNReportAllLevelsByMonth(DashboardPCNDto dto);

    Task<List<ReportDashboardPayeshPCN>> GetDashboardReportAllMonthsByLevelIdResult(DashboardPCNDto dto);

    Task<List<ReportDashboardPayeshPCN>> GetDashboardReportAllLevelIsByMonthResult(DashboardPCNDto dto);

    Task GetPayeshPCNReportAllLevelsByMonthProcess(long planId, long month, string serviceName, string storedProcedure, HttpResponse response);

    Task GetPayeshPCNReportAllMonthByLevelIdProcess(long planId, long levelId, string serviceName, string storedProcedure, HttpResponse response);

    Task GetAllLevelScoreByActionProcess(long planId, long month, string serviceName, string storedProcedure, long fetchType, HttpResponse response);

    Task<HashSet<AllLevelScoreDto>> GetAllLevelScoreByActionResult(long planId, long month, long fetchType);

    Task<List<ActionPayeshInfoDto>?> FindAllActionPayeshByPlanIdAndLevelIdAndMonth(long? planId, long? levelId, long? month);

    Task<List<ActionPayeshInfoDto>?> FindAllActionPCNAchievedInfo(long? planId, long? levelId, long? month);

    Task<List<MeasurePayeshPCNInfoDto>?> FindAllMeasurePCNAchieveInfo(long? planId, long? levelId, long? month);

    Task<List<MeasurePayeshPCNInfoDto>?> FindAllMeasurePayeshInfo(long? planId, long? levelId, long? month);

    Task<List<StrategyMapEvalByMeasureHoscoDto>> StrategyMapEvalByMeasure_ByLevelId_Hosco(DashboardPCNDto dto);

    Task<List<StrategyMapEvalByMeasureHoscoDto>?> StrategyMapEvalByMeasure_ByOrganizationId_Hosco(DashboardPCNDto dto);

    Task<List<StrategyMapEvalByActionHoscoDto>?> StrategyMapEvalByAction_ByLevelId_Hosco(DashboardPCNDto dto);

    Task<List<StrategyMapEvalByActionHoscoDto>?> StrategyMapEvalByAction_ByOrganizationId_Hosco(DashboardPCNDto dto);

    Task<List<MeasureEvalDto>?> GetMeasureEvalDataByOrganizationByStrategyOBjectId(long? strategyObjectId, long? planId, long? organizationId, long? month);

    Task<List<MeasureEvalDto>?> GetMeasureEvalDataByStrategyOBjectId(long? strategyObjectId, long? planId, long? levelId, long? month);

    Task<List<ActionEvalDto>?> GetAllActionEvalDataByOrganizationByStrategyObject(long? strategyObjectId, long? planId, long? organizationId, long? month);

    Task<List<ActionEvalDto>?> GetAllActionEvalDataByStrategyObject(long? strategyObjectId, long? planId, long? levelId, long? month);

    Task<List<AllLevelScoreDto>?> GetAllLevelScoreByMeasure(long? planId, long? month, long? fetchType);

    Task<List<AllLevelScoreDto>?> GetAllMainLevelScoreByMeasure(long? planId, long? month, long? fetchType);

    Task<List<AllLevelScoreDto>?> GetAllMainLevelScoreByAction(long? planId, long? month, long? fetchType);

    Task<List<EvalScoreDto>?> GetStrategyMapEvalScoreForMeasureByLevelid_Hosco(DashboardPCNDto dto);

    Task<List<EvalScoreDto>?> GetStrategyMapEvalScoreForMeasureByOrganizationid_Hosco(DashboardPCNDto dto);

    Task<List<EvalScoreDto>?> GetStrategyMapEvalScoreForActionByLevelid_Hosco(DashboardPCNDto dto);

    Task<List<EvalScoreDto>?> GetStrategyMapEvalScoreForActionByOrganizationid_Hosco(DashboardPCNDto dto);

    Task<List<DashboardComprehensiveDto>?> GetAllDashboardComprehensive(long planId, long levelId, long month);
}
