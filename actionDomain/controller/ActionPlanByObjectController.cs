using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.repository;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.SecurityDomain.Tenancy;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.actionDomain.controller;

[ApiController]
public sealed class ActionPlanByObjectController(
    ActionPlanByObjectReader reader, ServiceRuntime runtime, PlaningLevelUserServiceImpl levels, TenantContext tenant) : ControllerBase
{
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public sealed class Input
    {
        public long? LevelId { get; set; }
        public long? PlanId { get; set; }
        public long? ObjectId { get; set; }
        public long? Status { get; set; }
        public string? Filter { get; set; }
    }

    [HttpPost("api/v1/actionPlan-by-object/page")]
    [CheckAccess]
    public Task<IActionResult> Page([FromBody] Input input, [FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] string? direction, CancellationToken ct)
        => ReadPage(input, page, size, orderBy, direction, false, ct);

    [HttpPost("api/v1/actionPlan-by-object-isplanned/page")]
    [CheckAccess]
    public Task<IActionResult> PlannedPage([FromBody] Input input, [FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] string? direction, CancellationToken ct)
        => ReadPage(input, page, size, orderBy, direction, true, ct);

    private async Task<IActionResult> ReadPage(Input input, int page, int size, string? orderBy,
        string? direction, bool plannedOnly, CancellationToken ct)
    {
        if (!tenant.HasTenant) return Forbid();
        if (input.LevelId is not long levelId || levelId <= 0 || input.PlanId is not long planId || planId <= 0 ||
            input.ObjectId is not long objectId || objectId <= 0 || page < 0 || size <= 0 ||
            (long)page * Math.Min(size, 200) > int.MaxValue ||
            (!string.IsNullOrWhiteSpace(orderBy) && !orderBy.Equals("id", StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrWhiteSpace(direction) && !direction.Equals("ASC", StringComparison.OrdinalIgnoreCase) && !direction.Equals("DESC", StringComparison.OrdinalIgnoreCase)))
            return BadRequest(new { message = "شناسه‌ها یا اطلاعات صفحه‌بندی معتبر نیست." });

        await levels.HasAccessInLevelIdAsync(levelId, runtime.User.Id, ct);
        var query = reader.Query(tenant.TenantId, runtime.User.Id, levelId, planId, objectId, input.Status, input.Filter);
        var total = await query.LongCountAsync(ct);
        var limit = Math.Min(size, 200);
        var sorted = string.Equals(direction, "DESC", StringComparison.OrdinalIgnoreCase)
            ? query.OrderByDescending(item => item.Id) : query.OrderBy(item => item.Id);
        var items = await sorted.Skip(page * limit).Take(limit).ToListAsync(ct);
        var catalogIds = items.Where(item => item.ActionPlanCatalogId.HasValue)
            .Select(item => item.ActionPlanCatalogId!.Value).Distinct().ToArray();
        var states = await reader.ReadPlanningStatesAsync(tenant.TenantId, catalogIds, ct);
        var activities = await reader.ReadActivitiesAsync(tenant.TenantId, catalogIds, ct);
        var entityIds = activities.Where(item => !string.IsNullOrEmpty(item.EntityId))
            .Select(item => item.EntityId!).Distinct().ToArray();
        var uploaded = entityIds.Length == 0 ? new HashSet<string>() : (await runtime.Db.Database.SqlQuery<string>($"""
            SELECT a.entity_id AS Value FROM attach.stored_attachment a
            WHERE a.resource = {"ACTION_PLAN_ACTIVITY"} AND a.file_id IS NOT NULL
            """).Where(id => entityIds.Contains(id)).ToListAsync(ct)).ToHashSet();
        foreach (var activity in activities)
            activity.HasUploadedFile = activity.EntityId is not null && uploaded.Contains(activity.EntityId);
        var byCatalog = activities.ToLookup(item => item.ActionPlanCatalogId);

        var upperCatalogIds = items.Where(item => item.SendToUpperLevels && item.ActionPlanCatalogId.HasValue)
            .Select(item => item.ActionPlanCatalogId!.Value).Distinct().ToArray();
        var upperStrategies = new Dictionary<long, SemBackDotNet10.actionDomain.model.dto.UpperLevelStrategyObjectDto>();
        if (upperCatalogIds.Length > 0)
        {
            var activePlan = await runtime.External.FindActivePlanPeriodAsync(ct);
            if (activePlan is null) return Conflict(new { message = "دورهٔ برنامه‌ریزی فعال یافت نشد." });
            upperStrategies = await reader.ReadUpperStrategiesAsync(tenant.TenantId, activePlan.PlanId, upperCatalogIds, ct);
        }

        var actionIds = items.Where(item => item.Id.HasValue).Select(item => item.Id!.Value).ToArray();
        Dictionary<long, List<SemBackDotNet10.actionDomain.model.dto.ActionPlanBeneficiaryDto>>? beneficiaries = null;
        Dictionary<long, List<SemBackDotNet10.actionDomain.model.dto.ActionPlanCoworkerDto>>? coworkers = null;
        if (actionIds.Length > 0 && runtime.TenantName.Equals("HOSCO", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                beneficiaries = await reader.ReadBeneficiariesAsync(tenant.TenantId, actionIds, ct);
                coworkers = await reader.ReadCoworkersAsync(tenant.TenantId, actionIds, ct);
            }
            catch (SqlException exception) when (exception.Number == 208)
            {
                return StatusCode(503, new { error = "ACTION_PLAN_SCHEMA_UNAVAILABLE", message = "جداول ذی‌نفعان یا همکاران برنامهٔ اقدام در پایگاه داده در دسترس نیست." });
            }
        }
        foreach (var item in items)
        {
            if (item.ActionPlanCatalogId is long catalogId)
            {
                item.IsPlanned = states.GetValueOrDefault(catalogId);
                item.Activities = byCatalog[catalogId].ToList();
                item.TopLevelStrategyObject = upperStrategies.GetValueOrDefault(catalogId);
            }
            else item.Activities = [];
            if (item.ActionPlanCatalogCode is not null && item.CommitteeCode is not null)
                item.ActionPlanCatalogCode += "-" + item.CommitteeCode;
            if (item.Id is long actionId && beneficiaries is not null && coworkers is not null)
            {
                item.ActionPlanBeneficiaryDtoList = beneficiaries.GetValueOrDefault(actionId) ?? [];
                item.ActionPlanCoworkerDtoList = coworkers.GetValueOrDefault(actionId) ?? [];
            }
        }

        if (plannedOnly)
        {
            items = items.Where(item => item.IsPlanned == 2).ToList();
            total = items.Count;
        }
        return Ok(new ServicePage<SemBackDotNet10.actionDomain.model.dto.ActionPlanDto>(items, total, page, limit));
    }
}
