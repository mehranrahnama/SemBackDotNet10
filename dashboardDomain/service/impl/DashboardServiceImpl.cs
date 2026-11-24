using System.Text.Json;
using Common.exception;
using PlanningLevelUserService = global::SemBackDotNet10.service.impl.PlaningLevelUserServiceImpl;
using SemBackDotNet10.Common.exception;
using global::Common.logging;
using global::Common.service.interf;
using SemBackDotNet10.dashboardDomain.model.dto;
using SemBackDotNet10.dashboardDomain.model.entity;
using SemBackDotNet10.dashboardDomain.repository;
using SemBackDotNet10.dashboardDomain.service.interf;
using SemBackDotNet10.measureDomain.model.dto;
using SemBackDotNet10.SecurityDomain.Service.Impl;
using StrategyMapEvalByMeasureHoscoDto = SemBackDotNet10.dashboardDomain.model.dto.StrategyMapEvalByMeasureHoscoDto;

namespace SemBackDotNet10.dashboardDomain.service.impl;

public class DashboardServiceImpl : IDashboardService
{
    private readonly IDashboardRepository _repository;
    private readonly IDashboardPCNRepository _pcnRepository;
    private readonly IDashboardReportAllLevelScoreRepository _allLevelScoreRepository;
    private readonly PlanningLevelUserService _planingLevelUserService;
    private readonly TaskExecutorService _taskExecutor;
    private readonly SemLogger _logger;

    public DashboardServiceImpl(
        IDashboardRepository repository,
        IDashboardPCNRepository pcnRepository,
        IDashboardReportAllLevelScoreRepository allLevelScoreRepository,
        PlanningLevelUserService planingLevelUserService,
        TaskExecutorService taskExecutor,
        SemLogger logger)
    {
        _repository = repository;
        _pcnRepository = pcnRepository;
        _allLevelScoreRepository = allLevelScoreRepository;
        _planingLevelUserService = planingLevelUserService;
        _taskExecutor = taskExecutor;
        _logger = logger;
    }

    // ---------- Dashboard CRUD ----------

    public async Task CreateOrDelete(DashboardTobeDto dashboardDto)
    {
        var toCreate = await ResolveAsync(dashboardDto);
        if (toCreate != null) await _repository.SaveAllAsync(new[] { toCreate });
    }

    public async Task CreateOrDeleteBatchDashboard(List<DashboardTobeDto> dashboardDtoList)
    {
        var toCreate = new List<Dashboard>();
        foreach (var dto in dashboardDtoList)
        {
            var created = await ResolveAsync(dto);
            if (created != null) toCreate.Add(created);
        }
        if (toCreate.Count > 0) await _repository.SaveAllAsync(toCreate);
    }

    /// <summary>موجود باشد حذف می‌شود و null برمی‌گردد؛ در غیر این صورت موجودیت جدید ساخته می‌شود.</summary>
    private async Task<Dashboard?> ResolveAsync(DashboardTobeDto dto)
    {
        var userId = SecurityUtils.GetCurrentUserId();
        dto.UserId = userId;
        Dashboard? existing = null;
        if (userId != null)
        {
            existing = dto.MeasureId is > 0
                ? await _repository.FindByUserAndMeasureAsync(userId.Value, dto.MeasureId.Value)
                : dto.ActionPlanId != null
                    ? await _repository.FindByUserAndActionPlanAsync(userId.Value, dto.ActionPlanId.Value)
                    : null;
        }

        if (existing != null)
        {
            await _repository.DeleteAsync(existing);
            return null;
        }

        // Java selects exactly one target: a measure takes precedence over an action plan.
        var measureId = dto.MeasureId is > 0 ? dto.MeasureId : null;
        var actionPlanId = measureId is null ? dto.ActionPlanId : null;
        if (measureId is long measure)
        {
            if (!await _repository.MeasureExistsAsync(measure))
                throw new BusinessException("error.measure.idIsNotValid");
        }
        else if (actionPlanId is not long action || action <= 0 || !await _repository.ActionPlanExistsAsync(action))
            throw new BusinessException("error.actionPlan.idIsNotValid");

        // Hibernate sets both timestamps on insert; EF does not do so for Dashboard.
        var now = DateTime.Now;
        return new Dashboard
        {
            CreatedDate = now,
            ModifiedDate = now,
            UserId = userId,
            MeasureId = measureId,
            ActionPlanId = actionPlanId,
            Deleted = false
        };
    }

    public async Task<List<object>> FindAllMeasureActPlanByDashboardUserId(long planId, long userId)
    {
        if (userId == 0) throw new BusinessException("error.dashboard.idIsNotValid");
        var actions = await FindAllActionActPlanByDashboardUserId(planId, userId);
        var measures = DbRowMapper.MapAll<DashboardDto>(
            await _repository.ExecAsync("DashboardGetAllMeasureActPlanDataByUserIdPlanId", planId, userId));
        var result = new List<object>();
        result.Add(actions);
        result.Add(measures);
        return result;
    }

    public async Task<List<DashboardActionPlanDto>> FindAllActionActPlanByDashboardUserId(long planId, long userId)
    {
        if (userId == 0) throw new BusinessException("error.dashboard.idIsNotValid");
        var rows = await _repository.ExecAsync("DashboardGetAllActionActPlanDataByUserIdPlanId", planId, userId);
        return DbRowMapper.MapAll<DashboardActionPlanDto>(rows);
    }

    // ---------- Counts ----------

    private async Task<long?> ScalarAsync(string procedure, params object?[] args)
    {
        var rows = await _repository.ExecAsync(procedure, args);
        var value = rows.FirstOrDefault()?.FirstValue();
        return value is null or DBNull ? null : Convert.ToInt64(value);
    }

    public async Task<CountReportDTO> GetCountReport(long planId) => new()
    {
        StrategyObjectCount = await ScalarAsync("CountActiveGoals", planId),
        MeasureCount = await ScalarAsync("CountActiveMeasures", planId),
        ActionCount = await ScalarAsync("CountActiveActions", planId),
        UserCount = await ScalarAsync("CountActiveUsers", planId),
        ActiveStrategyCount = await ScalarAsync("CountActiveStrategy", planId)
    };

    public async Task<CountReportDTO> GetCountReportByPlanIdAndLevelId(string? body)
    {
        if (string.IsNullOrWhiteSpace(body)) throw new BusinessException("error.dashboard.idIsNotValid");
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;
        if (!root.TryGetProperty("planId", out var p) || !root.TryGetProperty("levelId", out var l)
            || !p.TryGetInt64(out var planId) || !l.TryGetInt64(out var levelId))
            throw new BusinessException("error.dashboard.idIsNotValid");

        var userId = SecurityUtils.GetCurrentUserId() ?? 0;
        if (!await _planingLevelUserService.HasAccessInLevelIdAsync(levelId, userId))
            throw new BusinessException("error.access.denied");

        return new CountReportDTO
        {
            StrategyObjectCount = await ScalarAsync("CountActiveGoalsByPlanIdLevelId", planId, levelId),
            MeasureCount = await ScalarAsync("CountActiveMeasuresByPlanIdLevelId", planId, levelId),
            ActionCount = await ScalarAsync("CountActiveActionsByPlanIdLevelId", planId, levelId),
            UserCount = await ScalarAsync("CountActiveUsersByPlanIdLevelId", planId, levelId),
            ActiveStrategyCount = await ScalarAsync("CountActiveStrategyByLevelId", levelId)
        };
    }

    // ---------- Payesh PCN ----------

    private static List<FetchDashboardPCNDto> ToFetchDtos(List<DbRow> rows, long? fallbackMonth)
    {
        var list = DbRowMapper.MapAll<FetchDashboardPCNDto>(rows);
        for (var i = 0; i < list.Count; i++)
        {
            if (!rows[i].Has("month") || rows[i].Get("month") is null or DBNull)
                list[i].Month = fallbackMonth == null ? null : (int)fallbackMonth.Value;
        }
        return list;
    }

    public async Task<List<FetchDashboardPCNDto>> GetPayeshPCNReport(DashboardPCNDto dto)
    {
        var rows = await _repository.ExecAsync("Report_Dashboard_Payesh_PCN", dto.PlanId, dto.LevelId, dto.Month);
        return ToFetchDtos(rows, dto.Month);
    }

    public async Task<List<FetchDashboardPCNDto>> GetPayeshPCNReportAllLevelsByMonth(DashboardPCNDto dto)
    {
        var rows = await _repository.ExecAsync("Report_Dashboard_Payesh_PCN_AllLevelsByMonth", dto.PlanId, dto.Month);
        return ToFetchDtos(rows, dto.Month);
    }

    public Task<List<ReportDashboardPayeshPCN>> GetDashboardReportAllMonthsByLevelIdResult(DashboardPCNDto dto) =>
        _pcnRepository.FindByPlanIdAndLevelIdAsync(dto.PlanId ?? 0, dto.LevelId ?? 0);

    public Task<List<ReportDashboardPayeshPCN>> GetDashboardReportAllLevelIsByMonthResult(DashboardPCNDto dto) =>
        _pcnRepository.FindByPlanIdAndMonthAsync(dto.PlanId ?? 0, dto.Month ?? 0);

    // ---------- SSE processes ----------

    private async Task RunProcessAsync(string key, HttpResponse response, Func<Task> work)
    {
        _taskExecutor.Subscribe(key);
        try
        {
            await _taskExecutor.RunTaskAsync(key, _ => work(), response.HttpContext.RequestAborted);
        }
        catch (OperationCanceledException) when (response.HttpContext.RequestAborted.IsCancellationRequested)
        {
            // The client disconnected before the stream completed.
        }
        catch (Exception ex)
        {
            _logger.Error($"Dashboard process failed: {key}: {ex}");
        }
    }

    public Task GetPayeshPCNReportAllLevelsByMonthProcess(long planId, long month, string serviceName, string storedProcedure, HttpResponse response)
    {
        var key = $"{month}:{planId}:{SecurityUtils.GetCurrentUserName()}:{serviceName}";
        return RunProcessAsync(key, response, () => _pcnRepository.ExecuteReportDashboardPayeshPCNAllLevelsByMonthAsync(planId, month));
    }

    public Task GetPayeshPCNReportAllMonthByLevelIdProcess(long planId, long levelId, string serviceName, string storedProcedure, HttpResponse response)
    {
        var key = $"{levelId}:{planId}:{SecurityUtils.GetCurrentUserName()}:{serviceName}";
        return RunProcessAsync(key, response, () => _pcnRepository.ExecuteReportDashboardPayeshPCNAllMonthByLevelIdAsync(planId, levelId));
    }

    public Task GetAllLevelScoreByActionProcess(long planId, long month, string serviceName, string storedProcedure, long fetchType, HttpResponse response)
    {
        var key = $"{month}:{planId}:{SecurityUtils.GetCurrentUserName()}:{serviceName}";
        return RunProcessAsync(key, response, () => _pcnRepository.ExecuteReportAllLevelScoreByActionHoscoOnlineAsync(planId, month, fetchType));
    }

    public Task<HashSet<AllLevelScoreDto>> GetAllLevelScoreByActionResult(long planId, long month, long fetchType) =>
        _allLevelScoreRepository.FindAllLevelScoreAsync(planId, month, fetchType);

    // ---------- Stored procedure reports ----------

    private async Task<List<T>?> QueryAsync<T>(string procedure, params object?[] args) where T : new()
    {
        var rows = await _repository.ExecAsync(procedure, args);
        return rows.Count == 0 ? null : DbRowMapper.MapAll<T>(rows);
    }

    public Task<List<ActionPayeshInfoDto>?> FindAllActionPayeshByPlanIdAndLevelIdAndMonth(long? planId, long? levelId, long? month) =>
        QueryAsync<ActionPayeshInfoDto>("Dashboard_ActionPayeshInfo_ByLevelId_ByMonth", planId, levelId, month);

    public Task<List<ActionPayeshInfoDto>?> FindAllActionPCNAchievedInfo(long? planId, long? levelId, long? month) =>
        QueryAsync<ActionPayeshInfoDto>("Dashboard_ActionPCNAchieveInfo_ByLevelId_ByMonth", planId, levelId, month);

    public Task<List<MeasurePayeshPCNInfoDto>?> FindAllMeasurePCNAchieveInfo(long? planId, long? levelId, long? month) =>
        QueryAsync<MeasurePayeshPCNInfoDto>("Dashboard_MeasurePCNAchieveInfo_ByLevelId_ByMonth", planId, levelId, month);

    public Task<List<MeasurePayeshPCNInfoDto>?> FindAllMeasurePayeshInfo(long? planId, long? levelId, long? month) =>
        QueryAsync<MeasurePayeshPCNInfoDto>("Dashboard_MeasurePayeshInfo_ByLevelId_ByMonth", planId, levelId, month);

    public async Task<List<StrategyMapEvalByMeasureHoscoDto>> StrategyMapEvalByMeasure_ByLevelId_Hosco(DashboardPCNDto dto) =>
        await QueryAsync<StrategyMapEvalByMeasureHoscoDto>("Report_StrategyMap_Eval_ByMeasure_ByLevelId_Hosco", dto.PlanId, dto.LevelId, dto.Month)
        ?? new List<StrategyMapEvalByMeasureHoscoDto>();

    public Task<List<StrategyMapEvalByMeasureHoscoDto>?> StrategyMapEvalByMeasure_ByOrganizationId_Hosco(DashboardPCNDto dto) =>
        QueryAsync<StrategyMapEvalByMeasureHoscoDto>("Report_StrategyMap_Eval_ByMeasure_ByOrganizationId_Hosco", dto.PlanId, dto.OrganizationId, dto.Month);

    public Task<List<StrategyMapEvalByActionHoscoDto>?> StrategyMapEvalByAction_ByLevelId_Hosco(DashboardPCNDto dto) =>
        QueryAsync<StrategyMapEvalByActionHoscoDto>("Report_StrategyMap_Eval_ByAction_ByLevelId_Hosco", dto.PlanId, dto.LevelId, dto.Month);

    public Task<List<StrategyMapEvalByActionHoscoDto>?> StrategyMapEvalByAction_ByOrganizationId_Hosco(DashboardPCNDto dto) =>
        QueryAsync<StrategyMapEvalByActionHoscoDto>("Report_StrategyMap_Eval_ByAction_ByOrganizationId_Hosco", dto.PlanId, dto.OrganizationId, dto.Month);

    public Task<List<MeasureEvalDto>?> GetMeasureEvalDataByOrganizationByStrategyOBjectId(long? strategyObjectId, long? planId, long? organizationId, long? month) =>
        QueryAsync<MeasureEvalDto>("Report_Measure_EvalData_ByStrategyObjectId_ByOrganizationId_Hosco", strategyObjectId, planId, organizationId, month);

    public Task<List<MeasureEvalDto>?> GetMeasureEvalDataByStrategyOBjectId(long? strategyObjectId, long? planId, long? levelId, long? month) =>
        QueryAsync<MeasureEvalDto>("Report_Measure_EvalData_ByStrategyObjectId_Hosco", strategyObjectId, planId, levelId, month);

    public Task<List<ActionEvalDto>?> GetAllActionEvalDataByOrganizationByStrategyObject(long? strategyObjectId, long? planId, long? organizationId, long? month) =>
        QueryAsync<ActionEvalDto>("Report_Action_EvalData_ByOrganizationId_ByStrategyObjectId_Hosco", strategyObjectId, planId, organizationId, month);

    public Task<List<ActionEvalDto>?> GetAllActionEvalDataByStrategyObject(long? strategyObjectId, long? planId, long? levelId, long? month) =>
        QueryAsync<ActionEvalDto>("Report_Action_EvalData_ByStrategyObjectId_Hosco_MainLevel", strategyObjectId, planId, levelId, month);

    public Task<List<AllLevelScoreDto>?> GetAllLevelScoreByMeasure(long? planId, long? month, long? fetchType) =>
        QueryAsync<AllLevelScoreDto>("Report_AllLevelScore_ByMeasure_Hosco", planId, month, fetchType);

    public Task<List<AllLevelScoreDto>?> GetAllMainLevelScoreByMeasure(long? planId, long? month, long? fetchType) =>
        QueryAsync<AllLevelScoreDto>("Report_AllMainLevelScore_ByMeasure_Hosco", planId, month, fetchType);

    public Task<List<AllLevelScoreDto>?> GetAllMainLevelScoreByAction(long? planId, long? month, long? fetchType) =>
        QueryAsync<AllLevelScoreDto>("Report_AllMainLevelScore_ByAction_Hosco", planId, month, fetchType);

    public Task<List<EvalScoreDto>?> GetStrategyMapEvalScoreForMeasureByLevelid_Hosco(DashboardPCNDto dto) =>
        QueryAsync<EvalScoreDto>("Report_StrategyMap_EvalScore_ForMeasure_ByLevelid_Hosco", dto.PlanId, dto.LevelId, dto.Month);

    public Task<List<EvalScoreDto>?> GetStrategyMapEvalScoreForMeasureByOrganizationid_Hosco(DashboardPCNDto dto) =>
        QueryAsync<EvalScoreDto>("Report_StrategyMap_EvalScore_ForMeasure_ByOrganizationId_Hosco", dto.PlanId, dto.OrganizationId, dto.Month);

    public Task<List<EvalScoreDto>?> GetStrategyMapEvalScoreForActionByLevelid_Hosco(DashboardPCNDto dto) =>
        QueryAsync<EvalScoreDto>("Report_StrategyMap_EvalScore_ForAction_ByLevelid_Hosco", dto.PlanId, dto.LevelId, dto.Month);

    public Task<List<EvalScoreDto>?> GetStrategyMapEvalScoreForActionByOrganizationid_Hosco(DashboardPCNDto dto) =>
        QueryAsync<EvalScoreDto>("Report_StrategyMap_EvalScore_ForAction_ByOrganizationId_Hosco", dto.PlanId, dto.OrganizationId, dto.Month);

    public Task<List<DashboardComprehensiveDto>?> GetAllDashboardComprehensive(long planId, long levelId, long month) =>
        QueryAsync<DashboardComprehensiveDto>("Dashboard_comprehensive", planId, levelId, month);
}
