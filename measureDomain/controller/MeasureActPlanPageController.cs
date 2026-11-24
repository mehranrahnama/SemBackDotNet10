using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.SecurityDomain.Tenancy;
using SemBackDotNet10.measureDomain.controller.dto.measureActPlanDtos;
using SemBackDotNet10.measureDomain.model.dto;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.measureDomain.controller;

[ApiController]
[Route("api/v1/measureActPlan/page")]
public sealed class MeasureActPlanPageController(ServiceRuntime runtime, PlaningLevelUserServiceImpl levels, TenantContext tenant) : ControllerBase
{
    [HttpPost("~/api/v1/customMeasureActPlan/page")]
    [CheckAccess]
    public async Task<IActionResult> CustomPage([FromBody] MeasureActPlanRequestDto input, [FromQuery] int page,
        [FromQuery] int size, [FromQuery] string? orderBy, [FromQuery] string? direction, CancellationToken ct)
    {
        if (!tenant.HasTenant) return Forbid();
        if (input.MeasureId is not long measureId || measureId <= 0 || page < 0 || size <= 0 ||
            (long)page * Math.Min(size, 200) > int.MaxValue ||
            (!string.IsNullOrWhiteSpace(orderBy) && !orderBy.Equals("id", StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(direction) && !direction.Equals("ASC", StringComparison.OrdinalIgnoreCase) && !direction.Equals("DESC", StringComparison.OrdinalIgnoreCase)))
            return BadRequest(new { message = "شناسه یا اطلاعات صفحه‌بندی معتبر نیست." });

        var levelId = await runtime.Db.Database.SqlQuery<long>($"""
            SELECT CAST(l.levelid AS bigint) AS Value
            FROM dbo.tbldimmeasure m
            JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
            JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
            WHERE m.measureid = {measureId} AND m.isdeleted = 0 AND l.tenantid = {tenant.TenantId}
            """).SingleOrDefaultAsync(ct);
        if (levelId == 0) return NotFound();
        await levels.HasAccessInLevelIdAsync(levelId, runtime.User.Id, ct);

        var limit = Math.Min(size, 200);
        return Ok(await ReadPage(measureId, false, page, limit, page * limit, direction, ct));
    }

    [HttpPost("~/api/v1/measureActPlan/initialPage")]
    [CheckAccess]
    public async Task<IActionResult> InitialPage([FromBody] MeasureActPlanRequestDto input, [FromQuery] int page,
        [FromQuery] int size, [FromQuery] string? orderBy, [FromQuery] string? direction, CancellationToken ct)
    {
        if (!tenant.HasTenant) return Forbid();
        if (input.MeasureId is not long measureId || measureId <= 0 || input.LevelId is not long levelId || levelId <= 0 ||
            input.PlanId is not long planId || planId <= 0 || page < 0 || size <= 0 ||
            (long)page * Math.Min(size, 200) > int.MaxValue ||
            (!string.IsNullOrWhiteSpace(orderBy) && !orderBy.Equals("id", StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(direction) && !direction.Equals("ASC", StringComparison.OrdinalIgnoreCase) && !direction.Equals("DESC", StringComparison.OrdinalIgnoreCase)))
            return BadRequest(new { message = "شناسه‌ها یا اطلاعات صفحه‌بندی معتبر نیست." });

        await levels.HasAccessInLevelIdAsync(levelId, runtime.User.Id, ct);
        string result;
        try { result = await InitializePlan(measureId, planId, levelId, input.CustomMonth, ct); }
        catch (MeasureAncestorConflictException exception) { return Conflict(new { message = exception.Message }); }
        catch (SqlException exception) when (exception.Number == 547)
        {
            return Conflict(new { message = "برنامهٔ شاخص وابستگی پایگاه داده دارد و امکان جایگزینی آن وجود ندارد." });
        }
        if (result == "missing") return NotFound();
        if (result == "invalid") return BadRequest(new { message = "هدف یا ماه‌های برنامه‌ریزی معتبر نیست." });
        if (result == "children") return Conflict(new { message = "شاخص‌های زیرمجموعه یا برنامهٔ ماهانهٔ آن‌ها کامل و یکتا نیست." });
        if (result != "ready") return StatusCode(503, new { error = "MEASURE_INITIAL_PLANNING_UNAVAILABLE", message = "ایجاد برنامهٔ اولیهٔ شاخص انجام نشد." });

        var limit = Math.Min(size, 200);
        return Ok(await ReadPage(measureId, false, page, limit, page * limit, direction, ct));
    }

    private Task<string> InitializePlan(long measureId, long planId, long levelId, string? customMonth, CancellationToken ct)
        => runtime.TransactionAsync(async () =>
        {
            var state = await runtime.Db.Database.SqlQuery<MeasureInitialPlanState>($"""
                SELECT CAST(c.measureperiod AS int) AS MeasurePeriod, CAST(l.parentid AS bigint) AS ParentId,
                       CAST(c.dimmeasurecatalogid AS bigint) AS CatalogId,
                       m.currentyeartarget AS CurrentYearTarget, CAST(m.isplanned AS bit) AS IsPlanned,
                       CAST(m.addtosupremelevel AS bit) AS AddToSupremeLevel,
                       CAST(CASE WHEN EXISTS (SELECT 1 FROM dbo.tbldimmeasurecatalogrelation rel WITH (HOLDLOCK)
                           WHERE rel.mainmeasurecatid = c.dimmeasurecatalogid) THEN 1 ELSE 0 END AS bit) AS HasRelations
                FROM dbo.tbldimmeasure m WITH (UPDLOCK, HOLDLOCK)
                JOIN dbo.tbldimmeasurecatalog c WITH (HOLDLOCK) ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
                JOIN dbo.tblplaninglevel l WITH (HOLDLOCK) ON l.levelid = c.levelid
                WHERE m.measureid = {measureId} AND m.planid = {planId} AND m.isdeleted = 0
                  AND l.levelid = {levelId} AND l.tenantid = {tenant.TenantId}
                """).SingleOrDefaultAsync(ct);
            if (state is null) return "missing";
            if (!MeasureDefaultPlanCalculator.TryGetActiveMonths(state.MeasurePeriod, false, customMonth, out var activeMonths)) return "invalid";
            if (!state.ParentId.HasValue && state.HasRelations && runtime.TenantName.Equals("IRISA", StringComparison.OrdinalIgnoreCase))
            {
                if (state.CurrentYearTarget is null) return "invalid";
                if (!MeasureDefaultPlanCalculator.TryGetActiveMonths(state.MeasurePeriod, state.IsPlanned, customMonth, out var weightedMonths)) return "invalid";
                var weighted = await ApplyWeightedPlan(measureId, planId, weightedMonths, ct);
                if (weighted != "ready") return weighted;
                if (state.AddToSupremeLevel) await new MeasureInitialAncestorWriter(runtime, tenant).CopyAsync(measureId, state.CatalogId, planId, ct);
                return "ready";
            }

            await DeleteCurrentPlan(measureId, ct);

            foreach (var month in Enumerable.Range(1, 12))
            {
                var active = activeMonths.Contains(month);
                await runtime.Db.Database.ExecuteSqlInterpolatedAsync($"""
                    INSERT INTO dbo.tbldimmeasureactplan
                      (dimmeasureid, month, active, confirmstatus, isdeleted, createddate, modifieddate, createdby, modifiedby, version)
                    VALUES ({measureId}, {month}, {active}, 0, 0, {DateTime.UtcNow}, {DateTime.UtcNow}, {runtime.User.Id}, {runtime.User.Id}, 0)
                    """, ct);
            }
            if (state.AddToSupremeLevel) await new MeasureInitialAncestorWriter(runtime, tenant).CopyAsync(measureId, state.CatalogId, planId, ct);
            return "ready";
        }, ct);

    private Task<int> DeleteCurrentPlan(long measureId, CancellationToken ct) => runtime.Db.Database.ExecuteSqlInterpolatedAsync($"""
        DECLARE @fileIds TABLE (file_id bigint PRIMARY KEY);
        INSERT INTO @fileIds (file_id)
        SELECT DISTINCT attachment.file_id
        FROM attach.stored_attachment attachment WITH (UPDLOCK, HOLDLOCK)
        JOIN dbo.tbldimmeasureactplan ap WITH (UPDLOCK, HOLDLOCK)
          ON attachment.entity_id = CONVERT(nvarchar(32), ap.dimmeasureactplanid)
        WHERE attachment.resource = N'MEASURE_ACT_PLAN' AND attachment.file_id IS NOT NULL
          AND ap.dimmeasureid = {measureId} AND ap.isdeleted = 0;

        DELETE attachment FROM attach.stored_attachment attachment
        JOIN dbo.tbldimmeasureactplan ap WITH (UPDLOCK, HOLDLOCK)
          ON attachment.entity_id = CONVERT(nvarchar(32), ap.dimmeasureactplanid)
        WHERE attachment.resource = N'MEASURE_ACT_PLAN' AND ap.dimmeasureid = {measureId} AND ap.isdeleted = 0;

        DELETE storedFile FROM attach.stored_file storedFile
        JOIN @fileIds files ON files.file_id = storedFile.file_id
        WHERE NOT EXISTS (SELECT 1 FROM attach.stored_attachment remaining WITH (UPDLOCK, HOLDLOCK)
                          WHERE remaining.file_id = storedFile.file_id);

        DELETE FROM dbo.tbldimmeasureactplan WHERE dimmeasureid = {measureId} AND isdeleted = 0;
        """, ct);

    [HttpPost]
    [CheckAccess]
    public async Task<IActionResult> Page([FromBody] MeasureActPlanRequestDto input, [FromQuery] int page,
        [FromQuery] int size, [FromQuery] string? orderBy, [FromQuery] string? direction, CancellationToken ct)
    {
        if (!tenant.HasTenant) return Forbid();
        if (input.MeasureId is not long measureId || measureId <= 0 || input.LevelId is not long levelId || levelId <= 0 ||
            input.PlanId is not long planId || planId <= 0 || page < 0 || size <= 0 ||
            (long)page * Math.Min(size, 200) > int.MaxValue ||
            (!string.IsNullOrWhiteSpace(orderBy) && !orderBy.Equals("id", StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(direction) && !direction.Equals("ASC", StringComparison.OrdinalIgnoreCase) && !direction.Equals("DESC", StringComparison.OrdinalIgnoreCase)))
            return BadRequest(new { message = "شناسه‌ها یا اطلاعات صفحه‌بندی معتبر نیست." });

        var state = await runtime.Db.Database.SqlQuery<MeasurePlanPageState>($"""
            SELECT CAST(m.isplanned AS bit) AS IsPlanned, CAST(m.replannedstatus AS bigint) AS ReplannedStatus,
                   m.currentyeartarget AS CurrentYearTarget, CAST(l.parentid AS bigint) AS ParentId,
                   CAST(CASE WHEN EXISTS (SELECT 1 FROM dbo.tbldimmeasurecatalogrelation relation
                       WHERE relation.mainmeasurecatid = c.dimmeasurecatalogid) THEN 1 ELSE 0 END AS bit) AS HasRelations
            FROM dbo.tbldimmeasure m
            JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
            JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
            WHERE m.measureid = {measureId} AND m.planid = {planId}
              AND l.levelid = {levelId} AND l.tenantid = {tenant.TenantId}
            """).SingleOrDefaultAsync(ct);
        if (state is null) return NotFound();
        await levels.HasAccessInLevelIdAsync(levelId, runtime.User.Id, ct);

        var limit = Math.Min(size, 200);
        var offset = page * limit;
        var pending = state.ReplannedStatus is long status && status != 0 && status != 20;
        if (pending)
        {
            var expired = await ReadPage(measureId, true, page, limit, offset, direction, ct);
            if (expired.TotalElements > 0) return Ok(expired);
        }
        if (state.CurrentYearTarget is null)
            return BadRequest(new { message = "هدف سال جاری برای شاخص مشخص نشده" });

        var active = await ReadPage(measureId, false, page, limit, offset, direction, ct);
        var defaultRoot = !state.HasRelations && new[] { "IRISA", "HOSCO", "SANGAN", "SEFIDDASH", "BASA" }
            .Contains(runtime.TenantName, StringComparer.OrdinalIgnoreCase);
        if (state.IsPlanned && active.TotalElements > 0 && (state.ParentId.HasValue || defaultRoot)) return Ok(active);

        if (state.ParentId.HasValue || defaultRoot)
        {
            var outcome = await CalculateDefaultPlan(measureId, planId, levelId, input.CustomMonth, ct);
            if (outcome == "invalid") return BadRequest(new { message = "مقادیر هدف یا ماه‌های برنامه‌ریزی معتبر نیست." });
            if (outcome == "asymmetric") return UnprocessableEntity(new { message = "ماه‌های فعال برنامهٔ نزولی سفارشی باید نسبت به ماه دوازدهم متقارن باشند." });
            if (outcome == "missing") return Conflict(new { message = "رکوردهای دوازده ماه برنامه‌ریزی کامل یا یکتا نیستند." });
            if (outcome == "expired") return Ok(await ReadPage(measureId, true, page, limit, offset, direction, ct));
            if (outcome == "ready") return Ok(await ReadPage(measureId, false, page, limit, offset, direction, ct));
        }
        else
        {
            string outcome;
            try { outcome = await CalculateWeightedPlan(measureId, planId, levelId, input.CustomMonth, ct); }
            catch (SqlException exception) when (exception.Number == 547)
            {
                return Conflict(new { message = "برنامهٔ شاخص وابستگی پایگاه داده دارد و امکان جایگزینی آن وجود ندارد." });
            }
            if (outcome == "invalid") return BadRequest(new { message = "مقادیر هدف یا ماه‌های برنامه‌ریزی معتبر نیست." });
            if (outcome == "children") return Conflict(new { message = "شاخص‌های زیرمجموعه یا برنامهٔ ماهانهٔ آن‌ها کامل و یکتا نیست." });
            if (outcome == "expired") return Ok(await ReadPage(measureId, true, page, limit, offset, direction, ct));
            if (outcome == "ready") return Ok(await ReadPage(measureId, false, page, limit, offset, direction, ct));
        }

        return StatusCode(503, new { error = "MEASURE_PLANNING_UNAVAILABLE", message = "محاسبه یا ایجاد برنامه‌ریزی شاخص هنوز پیکربندی نشده است." });
    }

    private Task<string> CalculateWeightedPlan(long measureId, long planId, long levelId, string? customMonth, CancellationToken ct)
        => runtime.TransactionAsync(async () =>
        {
            var state = await runtime.Db.Database.SqlQuery<MeasureWeightedPlanState>($"""
                SELECT CAST(c.measureperiod AS int) AS MeasurePeriod, CAST(l.parentid AS bigint) AS ParentId,
                       CAST(m.isplanned AS bit) AS IsPlanned, CAST(m.replannedstatus AS bigint) AS ReplannedStatus,
                       m.currentyeartarget AS CurrentYearTarget
                FROM dbo.tbldimmeasure m WITH (UPDLOCK, HOLDLOCK)
                JOIN dbo.tbldimmeasurecatalog c WITH (HOLDLOCK) ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
                JOIN dbo.tblplaninglevel l WITH (HOLDLOCK) ON l.levelid = c.levelid
                WHERE m.measureid = {measureId} AND m.planid = {planId} AND m.isdeleted = 0
                  AND l.levelid = {levelId} AND l.tenantid = {tenant.TenantId}
                """).SingleOrDefaultAsync(ct);
            if (state is null || state.ParentId.HasValue) return "unsupported";
            if (state.ReplannedStatus is long status && status != 0 && status != 20)
            {
                var expired = await runtime.Db.Database.SqlQuery<long>($"""
                    SELECT CAST(ap.dimmeasureactplanid AS bigint) AS Value
                    FROM dbo.tbldimmeasureactplan ap WITH (UPDLOCK, HOLDLOCK)
                    WHERE ap.dimmeasureid = {measureId} AND ap.isdeleted = 1 AND ap.active = 1
                    """).AnyAsync(ct);
                if (expired) return "expired";
            }
            if (state.CurrentYearTarget is null || !MeasureDefaultPlanCalculator.TryGetActiveMonths(state.MeasurePeriod, state.IsPlanned, customMonth, out var activeMonths)) return "invalid";
            return await ApplyWeightedPlan(measureId, planId, activeMonths, ct);
        }, ct);

    private async Task<string> ApplyWeightedPlan(long measureId, long planId, HashSet<int> activeMonths, CancellationToken ct)
    {
        var relations = await runtime.Db.Database.SqlQuery<MeasureWeightedRelation>($"""
            SELECT CAST(r.relatedmeasurecatid AS bigint) AS RelatedCatalogId, r.weight AS Weight,
                   CAST(l.tenantid AS bigint) AS RelatedTenantId
            FROM dbo.tbldimmeasurecatalogrelation r WITH (HOLDLOCK)
            JOIN dbo.tbldimmeasure m ON m.dimmeasurecatalogid = r.mainmeasurecatid
            LEFT JOIN dbo.tbldimmeasurecatalog c WITH (HOLDLOCK) ON c.dimmeasurecatalogid = r.relatedmeasurecatid
            LEFT JOIN dbo.tblplaninglevel l WITH (HOLDLOCK) ON l.levelid = c.levelid
            WHERE m.measureid = {measureId} AND r.planid = {planId}
            """).ToListAsync(ct);
        if (relations.Count == 0 || relations.Any(relation => relation.RelatedTenantId != tenant.TenantId)) return "children";

        var children = new List<WeightedMeasureChild>();
        foreach (var relation in relations)
        {
            var childIds = await runtime.Db.Database.SqlQuery<long>($"""
                SELECT CAST(child.measureid AS bigint) AS Value
                FROM dbo.tbldimmeasure child WITH (HOLDLOCK)
                WHERE child.dimmeasurecatalogid = {relation.RelatedCatalogId} AND child.planid = {planId} AND child.isdeleted = 0
                """).ToListAsync(ct);
            if (childIds.Count != 1 || childIds[0] == measureId) return "children";
            var childId = childIds[0];
            var months = await runtime.Db.Database.SqlQuery<MeasureWeightedMonth>($"""
                SELECT CAST(ap.month AS int) AS Month, ap.planvalue AS PlanValue, ap.monthplanvalue AS MonthPlanValue
                FROM dbo.tbldimmeasureactplan ap WITH (HOLDLOCK)
                WHERE ap.dimmeasureid = {childId} AND ap.isdeleted = 0
                """).ToListAsync(ct);
            if (months.Count != 12 || months.Any(month => month.Month is null or < 1 or > 12) ||
                months.Select(month => month.Month).Distinct().Count() != 12) return "children";
            children.Add(new WeightedMeasureChild(relation.Weight, months.ToDictionary(month => month.Month!.Value,
                month => new WeightedMeasureMonth(month.PlanValue, month.MonthPlanValue))));
        }
        if (!MeasureWeightedPlanCalculator.TryCalculate(children, activeMonths, out var values)) return "children";

        await DeleteCurrentPlan(measureId, ct);
        foreach (var month in Enumerable.Range(1, 12))
        {
            var value = values[month];
            var active = activeMonths.Contains(month);
            var planParameter = DecimalParameter("@planValue", value.Plan);
            var monthParameter = DecimalParameter("@monthPlanValue", value.MonthPlan);
            await runtime.Db.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO dbo.tbldimmeasureactplan
                  (dimmeasureid, month, active, planvalue, monthplanvalue, confirmstatus, isdeleted, createddate, modifieddate, createdby, modifiedby, version)
                VALUES ({measureId}, {month}, {active}, {planParameter}, {monthParameter}, 0, 0,
                        {DateTime.UtcNow}, {DateTime.UtcNow}, {runtime.User.Id}, {runtime.User.Id}, 0)
                """, ct);
        }
        if (runtime.TenantName.Equals("IRISA", StringComparison.OrdinalIgnoreCase))
            await runtime.Db.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE dbo.tbldimmeasure SET isplanned = 1, confirmstatus = 2, modifieddate = {DateTime.UtcNow},
                    modifiedby = {runtime.User.Id}, version = COALESCE(version, 0) + 1
                WHERE measureid = {measureId} AND planid = {planId}
                """, ct);
        return "ready";
    }

    private Task<string> CalculateDefaultPlan(long measureId, long planId, long levelId, string? customMonth, CancellationToken ct)
        => runtime.TransactionAsync(async () =>
        {
            var state = await runtime.Db.Database.SqlQuery<MeasureDefaultPlanState>($"""
                SELECT CAST(m.isplanned AS bit) AS IsPlanned, CAST(m.replannedstatus AS bigint) AS ReplannedStatus,
                       m.currentvalue AS CurrentValue, m.currentyeartarget AS CurrentYearTarget,
                       CAST(m.zeroinitial AS bit) AS ZeroInitial, CAST(l.parentid AS bigint) AS ParentId,
                       CAST(c.measureperiod AS int) AS MeasurePeriod, CAST(r.desiredmethod AS int) AS DesiredMethod,
                       CAST(r.measurepayeshtype AS int) AS PayeshType,
                       CAST(CASE WHEN EXISTS (SELECT 1 FROM dbo.tbldimmeasurecatalogrelation relation WITH (HOLDLOCK)
                           WHERE relation.mainmeasurecatid = c.dimmeasurecatalogid) THEN 1 ELSE 0 END AS bit) AS HasRelations
                FROM dbo.tbldimmeasure m WITH (UPDLOCK, HOLDLOCK)
                JOIN dbo.tbldimmeasurecatalog c WITH (HOLDLOCK) ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
                JOIN dbo.tblDimMeasureReporsitory r WITH (HOLDLOCK) ON r.dimmeasurerepositoryid = c.dimmeasurecatalogrepositoryid
                JOIN dbo.tblplaninglevel l WITH (HOLDLOCK) ON l.levelid = c.levelid
                WHERE m.measureid = {measureId} AND m.planid = {planId}
                  AND l.levelid = {levelId} AND l.tenantid = {tenant.TenantId}
                """).SingleOrDefaultAsync(ct);
            if (state is null) return "unsupported";
            var defaultRoot = !state.HasRelations && new[] { "IRISA", "HOSCO", "SANGAN", "SEFIDDASH", "BASA" }
                .Contains(runtime.TenantName, StringComparer.OrdinalIgnoreCase);
            if (!state.ParentId.HasValue && !defaultRoot) return "unsupported";

            var rows = await runtime.Db.Database.SqlQuery<MeasureDefaultPlanRow>($"""
                SELECT CAST(ap.dimmeasureactplanid AS bigint) AS Id, CAST(ap.month AS int) AS Month,
                       CAST(ap.active AS bit) AS Active, CAST(ap.isdeleted AS bit) AS Deleted
                FROM dbo.tbldimmeasureactplan ap WITH (UPDLOCK, HOLDLOCK)
                WHERE ap.dimmeasureid = {measureId}
                """).ToListAsync(ct);
            if (state.ReplannedStatus is long status && status != 0 && status != 20 && rows.Any(row => row.Deleted == true && row.Active)) return "expired";
            if (state.IsPlanned && rows.Any(row => row.Deleted == false && row.Active)) return "ready";
            if (state.CurrentYearTarget is not decimal target) return "invalid";
            var otherPayeshType = state.PayeshType is not (1 or 2 or 3);
            if (!otherPayeshType && state.DesiredMethod is not (1 or 2 or 3 or 4 or 5 or 6 or 7)) return "unsupported";

            if (!MeasureDefaultPlanCalculator.TryGetActiveMonths(state.MeasurePeriod, state.IsPlanned, customMonth, out var activeMonths)) return "invalid";

            var current = state.ZeroInitial ? 0m : state.CurrentValue ?? 0m;
            var keepState = !otherPayeshType && state.DesiredMethod is 3 or 6 or 7;
            var descending = !otherPayeshType && state.DesiredMethod is 2 or 5;
            if (!otherPayeshType && !keepState && !descending && current > target) return "invalid";
            if (descending && current != 0m && current < target &&
                !new[] { "HOSCO", "IRISA" }.Contains(runtime.TenantName, StringComparer.OrdinalIgnoreCase)) return "invalid";
            if (descending && !MeasureDefaultPlanCalculator.HasSymmetricMonths(activeMonths)) return "asymmetric";
            var currentRows = rows.Where(row => row.Deleted == false).ToArray();
            if (currentRows.Length != 12 || currentRows.Any(row => row.Month is null or < 1 or > 12) ||
                currentRows.Select(row => row.Month).Distinct().Count() != 12) return "missing";

            var rowsByMonth = currentRows.ToDictionary(row => row.Month!.Value);
            foreach (var month in Enumerable.Range(1, 12))
            {
                var active = !otherPayeshType && activeMonths.Contains(month);
                var targetMonth = MeasureDefaultPlanCalculator.DestinationMonth(month, active, descending);
                var row = rowsByMonth[targetMonth];
                var (plan, monthPlan) = MeasureDefaultPlanCalculator.Calculate(month, active, current, target, state.PayeshType, keepState, descending);
                var planParameter = DecimalParameter("@planValue", plan);
                var monthParameter = DecimalParameter("@monthPlanValue", monthPlan);
                await runtime.Db.Database.ExecuteSqlInterpolatedAsync($"""
                    UPDATE dbo.tbldimmeasureactplan SET planvalue = {planParameter}, monthplanvalue = {monthParameter},
                        active = {active}, confirmstatus = 0, modifieddate = {DateTime.UtcNow},
                        modifiedby = {runtime.User.Id}, version = COALESCE(version, 0) + 1
                    WHERE dimmeasureactplanid = {row.Id} AND dimmeasureid = {measureId} AND isdeleted = 0
                    """, ct);
            }
            return "ready";
        }, ct);

    private static SqlParameter DecimalParameter(string name, decimal? value) => new(name, SqlDbType.Decimal)
    {
        Precision = 22, Scale = 4, Value = value.HasValue ? value.Value : DBNull.Value
    };

    private async Task<ServicePage<MeasureActPlanDto>> ReadPage(long measureId, bool deleted, int page,
        int size, int offset, string? direction, CancellationToken ct)
    {
        var query = runtime.Db.Database.SqlQuery<MeasurePlanPageRow>($"""
            SELECT CAST(ap.dimmeasureactplanid AS bigint) AS Id, CAST(ap.month AS bigint) AS Month,
                   ap.planvalue AS PlanValue, ap.monthplanvalue AS MonthPlanValue, ap.actualvalue AS ActualValue,
                   ap.dailyvalue AS DailyValue, CAST(m.isplanned AS bit) AS IsPlanned,
                   CAST(period.planid AS bigint) AS PlanId, CAST(m.measureid AS bigint) AS DimMeasureId,
                   CAST(ap.active AS bit) AS Active
            FROM dbo.tbldimmeasureactplan ap
            JOIN dbo.tbldimmeasure m ON m.measureid = ap.dimmeasureid
            JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
            JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
            LEFT JOIN dbo.tblplanperiod period ON period.planid = m.planid
            WHERE m.measureid = {measureId} AND l.tenantid = {tenant.TenantId}
              AND ap.active = 1 AND ap.isdeleted = {deleted}
            """);
        var total = await query.LongCountAsync(ct);
        var sorted = string.Equals(direction, "DESC", StringComparison.OrdinalIgnoreCase)
            ? query.OrderBy(row => row.Month).ThenByDescending(row => row.Id)
            : query.OrderBy(row => row.Month).ThenBy(row => row.Id);
        var rows = await sorted.Skip(offset).Take(size).ToListAsync(ct);
        var content = rows.Select(row => new MeasureActPlanDto
        {
            Id = row.Id, Month = row.Month, PlanValue = row.PlanValue, MonthPlanValue = row.MonthPlanValue,
            ActualValue = row.ActualValue, DailyValue = row.DailyValue, IsPlanned = row.IsPlanned,
            PlanId = row.PlanId, DimMeasureId = row.DimMeasureId, Active = row.Active
        }).ToArray();
        return new ServicePage<MeasureActPlanDto>(content, total, page, size);
    }
}

public sealed class MeasureInitialPlanState
{
    public long CatalogId { get; set; }
    public int MeasurePeriod { get; set; }
    public decimal? CurrentYearTarget { get; set; }
    public bool IsPlanned { get; set; }
    public long? ParentId { get; set; }
    public bool AddToSupremeLevel { get; set; }
    public bool HasRelations { get; set; }
}

public sealed class MeasureWeightedPlanState
{
    public int MeasurePeriod { get; set; }
    public long? ParentId { get; set; }
    public bool IsPlanned { get; set; }
    public long? ReplannedStatus { get; set; }
    public decimal? CurrentYearTarget { get; set; }
}

public sealed class MeasureWeightedRelation
{
    public long RelatedCatalogId { get; set; }
    public decimal Weight { get; set; }
    public long? RelatedTenantId { get; set; }
}

public sealed class MeasureWeightedMonth
{
    public int? Month { get; set; }
    public decimal? PlanValue { get; set; }
    public decimal? MonthPlanValue { get; set; }
}

public sealed class MeasureDefaultPlanState
{
    public bool IsPlanned { get; set; }
    public long? ReplannedStatus { get; set; }
    public decimal? CurrentValue { get; set; }
    public decimal? CurrentYearTarget { get; set; }
    public bool ZeroInitial { get; set; }
    public long? ParentId { get; set; }
    public int MeasurePeriod { get; set; }
    public int DesiredMethod { get; set; }
    public int PayeshType { get; set; }
    public bool HasRelations { get; set; }
}

public sealed class MeasureDefaultPlanRow
{
    public long Id { get; set; }
    public int? Month { get; set; }
    public bool Active { get; set; }
    public bool? Deleted { get; set; }
}

public sealed class MeasurePlanPageState
{
    public bool IsPlanned { get; set; }
    public long? ReplannedStatus { get; set; }
    public decimal? CurrentYearTarget { get; set; }
    public long? ParentId { get; set; }
    public bool HasRelations { get; set; }
}

public sealed class MeasurePlanPageRow
{
    public long Id { get; set; }
    public long? Month { get; set; }
    public decimal? PlanValue { get; set; }
    public decimal? MonthPlanValue { get; set; }
    public decimal? ActualValue { get; set; }
    public decimal? DailyValue { get; set; }
    public bool IsPlanned { get; set; }
    public long? PlanId { get; set; }
    public long DimMeasureId { get; set; }
    public bool Active { get; set; }
}
