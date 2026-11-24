using System.Text.Json.Serialization;
using Common.model.dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Tenancy;
using SemBackDotNet10.service.impl;

namespace Common.controller;

[ApiController]
[Route("api/v1/attachment/page")]
[Authorize(Policy = "SemRequestUriAccess")]
public sealed class AttachmentPageController(ServiceRuntime runtime, AttachmentOwnerAccess ownerAccess, TenantContext tenant) : ControllerBase
{
    public sealed class Input
    {
        public string? EntityName { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long? EntityId { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> Load([FromBody] Input input, [FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] string? direction, CancellationToken ct)
    {
        if (!tenant.HasTenant) return Forbid();
        if (input.EntityId is not long entityId || entityId <= 0 || string.IsNullOrWhiteSpace(input.EntityName) ||
            page < 0 || size <= 0 || (long)page * Math.Min(size, 200) > int.MaxValue)
            return BadRequest(new { message = "شناسه، نوع پیوست و اطلاعات صفحه‌بندی معتبر الزامی است." });
        if (orderBy is not null && !orderBy.Equals("id", StringComparison.OrdinalIgnoreCase) ||
            direction is not null && !direction.Equals("ASC", StringComparison.OrdinalIgnoreCase) && !direction.Equals("DESC", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "مرتب‌سازی درخواستی معتبر نیست." });

        var entityName = input.EntityName;
        var owner = await ownerAccess.CheckAsync(entityName, entityId, ct);
        if (owner == AttachmentOwnerResult.Unsupported)
            return UnprocessableEntity(new { error = "ATTACHMENT_ENTITY_UNSUPPORTED", message = "دسترسی به این نوع پیوست هنوز پیکربندی نشده است." });
        if (owner == AttachmentOwnerResult.NotFound) return NotFound();

        var query = runtime.Db.Database.SqlQuery<AttachmentPageRow>($"""
            SELECT CAST(a.attachmentid AS bigint) AS Id, a.filename AS FileName, a.des AS Des,
                   CAST(a.createdby AS bigint) AS CreatedBy
            FROM dbo.attachment a
            WHERE a.entityname = {entityName} AND a.entityid = {entityId} AND a.deleted = 0
            """);
        var total = await query.LongCountAsync(ct);
        var limit = Math.Min(size, 200);
        var sorted = string.Equals(direction, "DESC", StringComparison.OrdinalIgnoreCase)
            ? query.OrderByDescending(row => row.Id) : query.OrderBy(row => row.Id);
        var rows = await sorted.Skip(page * limit).Take(limit).ToListAsync(ct);
        var content = rows.Select(row => new AttachmentDto { Id = row.Id, FileName = row.FileName, Des = row.Des, CreatedBy = row.CreatedBy }).ToArray();
        return Ok(new ServicePage<AttachmentDto>(content, total, page, limit));
    }
}

public enum AttachmentOwnerResult { Allowed, NotFound, Unsupported }

public sealed class AttachmentOwnerAccess(ServiceRuntime runtime, PlaningLevelUserServiceImpl levels, TenantContext tenant)
{
    public async Task<AttachmentOwnerResult> CheckAsync(string entityName, long entityId, CancellationToken ct)
    {
        if (!tenant.HasTenant) return AttachmentOwnerResult.NotFound;
        IQueryable<long> ownerLevels;
        if (entityName == "Connect_Measure_To_Goal")
            ownerLevels = runtime.Db.Database.SqlQuery<long>($"""
                SELECT CAST(c.levelid AS bigint) AS [Value] FROM dbo.tbldimmeasurecatalog c
                JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
                WHERE c.dimmeasurecatalogid = {entityId} AND l.tenantid = {tenant.TenantId}
                """);
        else if (entityName == "Connect_Action_To_Goal")
            ownerLevels = runtime.Db.Database.SqlQuery<long>($"""
                SELECT CAST(c.levelid AS bigint) AS [Value] FROM dbo.tblactionplancatalog c
                JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
                WHERE c.actionplancatalogid = {entityId} AND l.tenantid = {tenant.TenantId}
                """);
        else if (entityName is "MEASURE_ACT_PLAN" or "MeasureActPlan")
            ownerLevels = runtime.Db.Database.SqlQuery<long>($"""
                SELECT CAST(c.levelid AS bigint) AS [Value] FROM dbo.tbldimmeasureactplan p
                JOIN dbo.tbldimmeasure m ON m.measureid = p.dimmeasureid
                JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
                JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
                WHERE p.dimmeasureactplanid = {entityId} AND l.tenantid = {tenant.TenantId}
                """);
        else if (entityName is "ACTION_ACT_PLAN" or "ActionActPlan")
            ownerLevels = runtime.Db.Database.SqlQuery<long>($"""
                SELECT CAST(c.levelid AS bigint) AS [Value] FROM dbo.tblactionactplan p
                JOIN dbo.tblactionplancatalog c ON c.actionplancatalogid = p.actionplancatalogid
                JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
                WHERE p.actionactplanid = {entityId} AND l.tenantid = {tenant.TenantId}
                """);
        else if (entityName == "ACTION_PLAN_ACTIVITY")
            ownerLevels = runtime.Db.Database.SqlQuery<long>($"""
                SELECT CAST(c.levelid AS bigint) AS [Value] FROM dbo.tblactionplanactivity activity
                JOIN dbo.tblactionplancatalog c ON c.actionplancatalogid = activity.actionplancatalogid
                JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
                WHERE activity.actionplanactivityid = {entityId} AND l.tenantid = {tenant.TenantId}
                """);
        else if (entityName == "Evaluation_measure_Details")
            ownerLevels = runtime.Db.Database.SqlQuery<long>($"""
                SELECT CAST(c.levelid AS bigint) AS [Value] FROM eval.tblevaluationmeasure evaluation
                JOIN dbo.tbldimmeasureactplan p ON p.dimmeasureactplanid = evaluation.dimmeasureactid
                JOIN dbo.tbldimmeasure m ON m.measureid = p.dimmeasureid
                JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
                JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
                WHERE evaluation.evaluationmeasureid = {entityId} AND l.tenantid = {tenant.TenantId}
                """);
        else if (entityName == "Evaluation_action_Details")
            ownerLevels = runtime.Db.Database.SqlQuery<long>($"""
                SELECT CAST(c.levelid AS bigint) AS [Value] FROM eval.tblevaluationaction evaluation
                JOIN dbo.tblactionactplan p ON p.actionactplanid = evaluation.actionactplanid
                JOIN dbo.tblactionplancatalog c ON c.actionplancatalogid = p.actionplancatalogid
                JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
                WHERE evaluation.evaluationactionid = {entityId} AND l.tenantid = {tenant.TenantId}
                """);
        else if (entityName == $"Corporation_{entityId}_Logo" || entityName == $"StrategyObjectJson_{entityId}")
            ownerLevels = runtime.Db.Database.SqlQuery<long>($"""
                SELECT CAST(l.levelid AS bigint) AS [Value] FROM dbo.tblplaninglevel l
                WHERE l.levelid = {entityId} AND l.tenantid = {tenant.TenantId}
                """);
        else
            return AttachmentOwnerResult.Unsupported;

        var levelId = await ownerLevels.SingleOrDefaultAsync(ct);
        if (levelId == 0) return AttachmentOwnerResult.NotFound;
        await levels.HasAccessInLevelIdAsync(levelId, runtime.User.Id, ct);
        return AttachmentOwnerResult.Allowed;
    }
}

public sealed class AttachmentPageRow
{
    public long Id { get; set; }
    public string? FileName { get; set; }
    public string? Des { get; set; }
    public long? CreatedBy { get; set; }
}
