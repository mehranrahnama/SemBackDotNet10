using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Tenancy;
using SemBackDotNet10.service.impl;

namespace Common.controller;

[ApiController]
[Route("api/v1/attachment")]
[Authorize(Policy = "SemRequestUriAccess")]
public sealed class AttachmentDownloadController(ServiceRuntime runtime, AttachmentOwnerAccess owners,
    TenantContext tenant, IConfiguration configuration, IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet("{filename}")]
    public async Task<IActionResult> Download(string filename, CancellationToken ct)
    {
        if (!tenant.HasTenant) return Forbid();
        if (!IsSafeFileName(filename)) return BadRequest();

        var rows = await runtime.Db.Database.SqlQuery<AttachmentDownloadRow>($"""
            SELECT a.filename AS FileName, a.entityname AS EntityName, CAST(a.entityid AS bigint) AS EntityId
            FROM dbo.attachment a WHERE a.filename = {filename} AND a.deleted = 0
            """).ToListAsync(ct);
        if (rows.Count != 1) return NotFound();
        var row = rows[0];
        if (row.EntityName is not string entityName || row.EntityId is not long id ||
            row.FileName is not string storedName || !IsSafeFileName(storedName)) return NotFound();
        var owner = await owners.CheckAsync(entityName, id, ct);
        if (owner == AttachmentOwnerResult.Unsupported)
            return UnprocessableEntity(new { error = "ATTACHMENT_ENTITY_UNSUPPORTED", message = "دسترسی به این نوع پیوست هنوز پیکربندی نشده است." });
        if (owner != AttachmentOwnerResult.Allowed) return NotFound();

        var configuredRoot = configuration["Storage:Location"] ?? "Upload";
        var root = Path.GetFullPath(configuredRoot, environment.ContentRootPath);
        var directory = new DirectoryInfo(root);
        if (!directory.Exists) return NotFound();
        for (var parent = directory; parent is not null; parent = parent.Parent)
            if ((parent.Attributes & FileAttributes.ReparsePoint) != 0) return StatusCode(503);
        var path = Path.Combine(root, storedName);
        var info = new FileInfo(path);
        if (!info.Exists || (info.Attributes & FileAttributes.ReparsePoint) != 0) return NotFound();
        try
        {
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize: 65536, options: FileOptions.Asynchronous | FileOptions.SequentialScan);
            return File(stream, "application/octet-stream", storedName);
        }
        catch (FileNotFoundException) { return NotFound(); }
        catch (DirectoryNotFoundException) { return NotFound(); }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        if (!tenant.HasTenant) return Forbid();
        if (id <= 0) return BadRequest();
        var row = await runtime.Db.Database.SqlQuery<AttachmentDeleteRow>($"""
            SELECT a.entityname AS EntityName, CAST(a.entityid AS bigint) AS EntityId,
                   CAST(COALESCE(a.version, 0) AS bigint) AS Version
            FROM dbo.attachment a WHERE a.attachmentid = {id} AND a.deleted = 0
            """).SingleOrDefaultAsync(ct);
        if (row?.EntityName is not string entityName || row.EntityId is not long entityId) return NotFound();
        var owner = await owners.CheckAsync(entityName, entityId, ct);
        if (owner == AttachmentOwnerResult.Unsupported)
            return UnprocessableEntity(new { error = "ATTACHMENT_ENTITY_UNSUPPORTED", message = "دسترسی به این نوع پیوست هنوز پیکربندی نشده است." });
        if (owner != AttachmentOwnerResult.Allowed) return NotFound();

        var now = TimeZoneInfo.ConvertTime(runtime.Clock.GetUtcNow(), TimeZoneInfo.FindSystemTimeZoneById("Asia/Tehran")).DateTime;
        var userId = runtime.User.Id;
        var updated = await runtime.Db.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE dbo.attachment SET deleted = 1, modifieddate = {now}, modifiedby = {userId}, version = COALESCE(version, 0) + 1
            WHERE attachmentid = {id} AND entityname = {entityName} AND entityid = {entityId}
              AND deleted = 0 AND COALESCE(version, 0) = {row.Version}
            """, ct);
        return updated == 1 ? Ok() : Conflict(new { error = "ATTACHMENT_CHANGED", message = "پیوست تغییر کرده است؛ دوباره تلاش کنید." });
    }

    private static bool IsSafeFileName(string name) =>
        !string.IsNullOrWhiteSpace(name) && name is not "." and not ".." &&
        name.IndexOfAny(['/', '\\', ':', '\0']) < 0 && !name.Any(char.IsControl) &&
        !Path.IsPathRooted(name) && Path.GetFileName(name) == name;
}

public sealed class AttachmentDownloadRow
{
    public string? FileName { get; set; }
    public string? EntityName { get; set; }
    public long? EntityId { get; set; }
}

public sealed class AttachmentDeleteRow
{
    public string? EntityName { get; set; }
    public long? EntityId { get; set; }
    public long Version { get; set; }
}
