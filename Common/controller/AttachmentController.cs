using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.SecurityDomain.Tenancy;
namespace Common.controller;
[ApiController]
[Route("api/v1")]
[Authorize(Policy="SemRequestUriAccess")]
public sealed class AttachmentController(IAttachmentUploadService service, AttachmentOwnerAccess owners, TenantContext tenant):ControllerBase
{
    [HttpPost("attachment")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm]List<IFormFile> file,[FromQuery]string name,[FromQuery]long id,
        [FromForm(Name="des")]string? des = null,[FromQuery(Name="des")]string? queryDes = null,
        CancellationToken ct = default)
    {
        if (!tenant.HasTenant) return Forbid();
        if (string.IsNullOrWhiteSpace(name) || id <= 0) return BadRequest();
        var owner = await owners.CheckAsync(name, id, ct);
        if (owner == AttachmentOwnerResult.Unsupported)
            return UnprocessableEntity(new { error = "ATTACHMENT_ENTITY_UNSUPPORTED", message = "دسترسی به این نوع پیوست هنوز پیکربندی نشده است." });
        if (owner != AttachmentOwnerResult.Allowed) return NotFound();
        try { return Ok(await service.CreateAsync(file, name, id, des ?? queryDes ?? string.Empty, ct)); }
        catch (AttachmentUploadValidationException exception) { return BadRequest(new { message = exception.Message }); }
    }
}
