using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using global::Common.service.interf;
using SemBackDotNet10.dashboardDomain.model.dto;
using SemBackDotNet10.dashboardDomain.service.interf;
using SemBackDotNet10.SecurityDomain.Service.Impl;

namespace SemBackDotNet10.dashboardDomain.controller;

[ApiController]
[Route("api/v1")]
[Authorize]
public class DashboardController : ControllerBase
{
    private const string ServiceDashboardPcnByMonth = "dashboardPCN_byMonth";
    private const string ServiceDashboardPcnByLevel = "dashboardPCN_byLevel";
    private const string ServiceAllLevelScoreByAction = "allLevelScore_byAction";
    private const string SpPcnAllLevelsByMonth = "dbo.Report_Dashboard_Payesh_PCN_AllLevelsByMonth";
    private const string SpPcnAllMonthByLevelId = "dbo.Report_Dashboard_Payesh_PCN_AllMonth_ByLevelId";
    private const string SpAllLevelScoreByActionOnline = "dbo.Report_AllLevelScore_ByAction_Hosco_Online";

    private readonly IDashboardService _service;
    private readonly TaskExecutorService _taskExecutor;

    public DashboardController(IDashboardService service, TaskExecutorService taskExecutor)
    {
        _service = service;
        _taskExecutor = taskExecutor;
    }

    private IActionResult OkOrNoContent<T>(T? result) where T : class =>
        result == null ? NoContent() : Ok(result);

    private static void PrepareSse(HttpResponse response)
    {
        response.ContentType = "text/event-stream";
        response.Headers.CacheControl = "no-cache";
    }

    [HttpPost("dashboard")]
    public async Task<IActionResult> CreateOrDelete([FromBody] DashboardTobeDto dto)
    {
        await _service.CreateOrDelete(dto);
        return Ok();
    }

    [HttpPost("dashboard/all")]
    public async Task<IActionResult> CreateOrDeleteBatch([FromBody] List<DashboardTobeDto> dtos)
    {
        await _service.CreateOrDeleteBatchDashboard(dtos);
        return Ok();
    }

    [HttpGet("dashboard/getDataByUserId")]
    public async Task<IActionResult> GetDataByUserId([FromQuery] long planId) =>
        Ok(await _service.FindAllMeasureActPlanByDashboardUserId(planId, SecurityUtils.GetCurrentUserId() ?? 0));

    [HttpGet("dashboard/getDataByUserId/actionPlan")]
    public async Task<IActionResult> GetActionPlanDataByUserId([FromQuery] long planId) =>
        Ok(await _service.FindAllActionActPlanByDashboardUserId(planId, SecurityUtils.GetCurrentUserId() ?? 0));

    [HttpGet("dashboard/countReport/{planId:long}")]
    public async Task<IActionResult> GetCountReport(long planId) => Ok(await _service.GetCountReport(planId));

    [HttpPost("dashboard/countReportByPlanIdLevelId")]
    public async Task<IActionResult> GetCountReportByPlanIdLevelId()
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        return Ok(await _service.GetCountReportByPlanIdAndLevelId(body));
    }

    // ---------- Payesh PCN ----------

    [HttpPost("dashboard/reportDashboardPayeshPCN")]
    public async Task<IActionResult> ReportDashboardPayeshPCN([FromBody] DashboardPCNDto dto)
    {
        var result = await _service.GetPayeshPCNReport(dto);
        return result.Count == 0 ? Ok() : Ok(result);
    }

    [HttpPost("dashboard/reportDashboardPayeshPCNAllMonthByLevelId/result")]
    public async Task<IActionResult> AllMonthByLevelIdResult([FromBody] DashboardPCNDto dto) =>
        Ok(await _service.GetDashboardReportAllMonthsByLevelIdResult(dto));

    [HttpGet("dashboard/reportDashboardPayeshPCNAllMonthByLevelId/process/{planId:long}/{levelId:long}")]
    public async Task AllMonthByLevelIdProcess(long planId, long levelId)
    {
        PrepareSse(Response);
        await _service.GetPayeshPCNReportAllMonthByLevelIdProcess(planId, levelId, ServiceDashboardPcnByLevel, SpPcnAllMonthByLevelId, Response);
    }

    [HttpGet("dashboard/reportDashboardPayeshPCNAllMonthByLevelId/status/{planId:long}/{levelId:long}")]
    public IActionResult AllMonthByLevelIdStatus(long planId, long levelId) =>
        Ok(_taskExecutor.GetTaskStatus(planId, levelId, null, SpPcnAllMonthByLevelId));

    [HttpPost("dashboard/reportDashboardPayeshPCNAllLevelsByMonth")]
    public async Task<IActionResult> ReportDashboardPayeshPCNAllLevelsByMonth([FromBody] DashboardPCNDto dto)
    {
        var result = await _service.GetPayeshPCNReportAllLevelsByMonth(dto);
        return result.Count == 0 ? Ok() : Ok(result);
    }

    [HttpPost("dashboard/reportDashboardPayeshPCNAllLevelsByMonth/result")]
    public async Task<IActionResult> AllLevelsByMonthResult([FromBody] DashboardPCNDto dto) =>
        Ok(await _service.GetDashboardReportAllLevelIsByMonthResult(dto));

    [HttpGet("dashboard/reportDashboardPayeshPCNAllLevelsByMonth/process/{planId:long}/{month:long}")]
    public async Task AllLevelsByMonthProcess(long planId, long month)
    {
        PrepareSse(Response);
        await _service.GetPayeshPCNReportAllLevelsByMonthProcess(planId, month, ServiceDashboardPcnByMonth, SpPcnAllLevelsByMonth, Response);
    }

    [HttpGet("dashboard/reportDashboardPayeshPCNAllLevelsByMonth/status/{planId:long}/{month:long}")]
    public IActionResult AllLevelsByMonthStatus(long planId, long month) =>
        Ok(_taskExecutor.GetTaskStatus(planId, null, month, SpPcnAllLevelsByMonth));

    // ---------- Action / Measure info ----------

    [HttpPost("dashboard/reportAllActionPayesh")]
    public async Task<IActionResult> ReportAllActionPayesh([FromBody] DashboardPCNDto dto) =>
        OkOrNoContent(await _service.FindAllActionPayeshByPlanIdAndLevelIdAndMonth(dto.PlanId, dto.LevelId, dto.Month));

    [HttpPost("dashboard/actionPCNAchieveInfo")]
    public async Task<IActionResult> ActionPCNAchieveInfo([FromBody] DashboardPCNDto dto) =>
        OkOrNoContent(await _service.FindAllActionPCNAchievedInfo(dto.PlanId, dto.LevelId, dto.Month));

    [HttpPost("dashboard/measurePCNAchieveInfo")]
    public async Task<IActionResult> MeasurePCNAchieveInfo([FromBody] DashboardPCNDto dto) =>
        OkOrNoContent(await _service.FindAllMeasurePCNAchieveInfo(dto.PlanId, dto.LevelId, dto.Month));

    [HttpPost("dashboard/measurePayeshInfo")]
    public async Task<IActionResult> MeasurePayeshInfo([FromBody] DashboardPCNDto dto) =>
        OkOrNoContent(await _service.FindAllMeasurePayeshInfo(dto.PlanId, dto.LevelId, dto.Month));

    // ---------- Strategy map ----------

    [HttpPost("dashboard/strategyMapEvalByMeasure_ByLevelId_Hosco")]
    public async Task<IActionResult> StrategyMapEvalByMeasureByLevelId([FromBody] DashboardPCNDto dto) =>
        Ok(await _service.StrategyMapEvalByMeasure_ByLevelId_Hosco(dto));

    [HttpPost("dashboard/strategyMapEvalByMeasure_ByOrganizationId_Hosco")]
    public async Task<IActionResult> StrategyMapEvalByMeasureByOrganizationId([FromBody] DashboardPCNDto dto) =>
        OkOrNoContent(await _service.StrategyMapEvalByMeasure_ByOrganizationId_Hosco(dto));

    [HttpPost("dashboard/strategyMapEvalByAction_ByLevelId_Hosco")]
    public async Task<IActionResult> StrategyMapEvalByActionByLevelId([FromBody] DashboardPCNDto dto) =>
        OkOrNoContent(await _service.StrategyMapEvalByAction_ByLevelId_Hosco(dto));

    [HttpPost("dashboard/strategyMapEvalByAction_ByOrganizationId_Hosco")]
    public async Task<IActionResult> StrategyMapEvalByActionByOrganizationId([FromBody] DashboardPCNDto dto) =>
        OkOrNoContent(await _service.StrategyMapEvalByAction_ByOrganizationId_Hosco(dto));

    [HttpPost("dashboard/measureEvalDataByStrategyObject")]
    public async Task<IActionResult> MeasureEvalDataByStrategyObject([FromBody] MeasureEvalDto dto) =>
        OkOrNoContent(dto.OrganizationId != null
            ? await _service.GetMeasureEvalDataByOrganizationByStrategyOBjectId(dto.StrategyObjectId, dto.PlanId, dto.OrganizationId, dto.Month)
            : await _service.GetMeasureEvalDataByStrategyOBjectId(dto.StrategyObjectId, dto.PlanId, dto.LevelId, dto.Month));

    [HttpPost("dashboard/ActionEvalDataByStrategyObject")]
    public async Task<IActionResult> ActionEvalDataByStrategyObject([FromBody] ActionEvalDto dto) =>
        OkOrNoContent(dto.OrganizationId != null
            ? await _service.GetAllActionEvalDataByOrganizationByStrategyObject(dto.StrategyObjectId, dto.PlanId, dto.OrganizationId, dto.Month)
            : await _service.GetAllActionEvalDataByStrategyObject(dto.StrategyObjectId, dto.PlanId, dto.LevelId, dto.Month));

    // ---------- All level score ----------

    [HttpPost("dashboard/AllLevelScoreByMeasure")]
    public async Task<IActionResult> AllLevelScoreByMeasure([FromBody] AllLevelScoreDto dto) =>
        OkOrNoContent(await _service.GetAllLevelScoreByMeasure(dto.PlanId, dto.Month, dto.FetchType));

    [HttpPost("dashboard/AllMainLevelScoreByMeasure")]
    public async Task<IActionResult> AllMainLevelScoreByMeasure([FromBody] AllLevelScoreDto dto) =>
        OkOrNoContent(await _service.GetAllMainLevelScoreByMeasure(dto.PlanId, dto.Month, dto.FetchType));

    [HttpPost("dashboard/AllMainLevelScoreByAction")]
    public async Task<IActionResult> AllMainLevelScoreByAction([FromBody] AllLevelScoreDto dto) =>
        OkOrNoContent(await _service.GetAllMainLevelScoreByAction(dto.PlanId, dto.Month, dto.FetchType));

    [HttpGet("dashboard/allLevelScoreByAction/result/{planId:long}/{month:long}/{fetchType:long}")]
    public async Task<IActionResult> AllLevelScoreByActionResult(long planId, long month, long fetchType) =>
        Ok(await _service.GetAllLevelScoreByActionResult(planId, month, fetchType));

    [HttpGet("dashboard/allLevelScoreByAction/process/{planId:long}/{month:long}/{fetchType:long}")]
    public async Task AllLevelScoreByActionProcess(long planId, long month, long fetchType)
    {
        PrepareSse(Response);
        await _service.GetAllLevelScoreByActionProcess(planId, month, ServiceAllLevelScoreByAction, SpAllLevelScoreByActionOnline, fetchType, Response);
    }

    [HttpGet("dashboard/allLevelScoreByAction/status/{planId:long}/{month:long}")]
    public IActionResult AllLevelScoreByActionStatus(long planId, long month) =>
        Ok(_taskExecutor.GetTaskStatus(planId, null, month, SpAllLevelScoreByActionOnline));

    // ---------- Eval score ----------

    [HttpPost("dashboard/getStrategyMapEvalScoreForMeasureByLevelid_Hosco")]
    public async Task<IActionResult> EvalScoreMeasureByLevel([FromBody] DashboardPCNDto dto) =>
        OkOrNoContent(await _service.GetStrategyMapEvalScoreForMeasureByLevelid_Hosco(dto));

    [HttpPost("dashboard/getStrategyMapEvalScoreForMeasureByOrganizationid_Hosco")]
    public async Task<IActionResult> EvalScoreMeasureByOrganization([FromBody] DashboardPCNDto dto) =>
        OkOrNoContent(await _service.GetStrategyMapEvalScoreForMeasureByOrganizationid_Hosco(dto));

    [HttpPost("dashboard/getStrategyMapEvalScoreForActionByLevelId")]
    public async Task<IActionResult> EvalScoreActionByLevel([FromBody] DashboardPCNDto dto) =>
        OkOrNoContent(await _service.GetStrategyMapEvalScoreForActionByLevelid_Hosco(dto));

    [HttpPost("dashboard/getStrategyMapEvalScoreForActionByOrganizationId")]
    public async Task<IActionResult> EvalScoreActionByOrganization([FromBody] DashboardPCNDto dto) =>
        OkOrNoContent(await _service.GetStrategyMapEvalScoreForActionByOrganizationid_Hosco(dto));

    [HttpGet("getAllDashboardComprehensive/{planId:long}/{levelId:long}/{month:long}")]
    public async Task<IActionResult> GetAllDashboardComprehensive(long planId, long levelId, long month)
    {
        var result = await _service.GetAllDashboardComprehensive(planId, levelId, month);
        return result == null ? Content(string.Empty) : Ok(result);
    }
}
