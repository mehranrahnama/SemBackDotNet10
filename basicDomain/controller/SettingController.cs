using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.Controllers;

[ApiController]
[Route("api")]
public class SettingController(SettingServiceImpl service, SemBackDotNet10.SecurityDomain.Tenancy.TenantContext tenant) : ControllerBase
{
    [HttpPost("v1/setting")]
    [CheckAccessIfAdmin]
    public async Task<IActionResult> SetSetting([FromHeader(Name = "factoryid")] long? tenantId,
        [FromBody] Setting setting, CancellationToken ct)
    {
        if (tenantId is null || tenantId != tenant.TenantId) return BadRequest();
        var saved = await service.CreateSettingAsync(tenantId.Value, setting.SettingKey, setting.SettingValue, ct);
        return saved is null ? StatusCode(417) : Ok(saved);
    }

    [HttpGet("v1/settingByTenant")]
    [CheckAccess]
    public async Task<IActionResult> GetSetting([FromHeader(Name = "factoryid")] long? tenantId, CancellationToken ct)
    {
        if (tenantId is null || tenantId != tenant.TenantId) return BadRequest();
        var settings = await service.GetAllByTenantIdAsync(tenantId.Value, ct);
        return settings.Count == 0 ? NoContent() : Ok(settings);
    }

    [HttpPut("v1/setting")]
    [CheckAccessIfAdmin]
    public async Task<IActionResult> UpdateSetting([FromHeader(Name = "factoryid")] long? tenantId,
        [FromBody] Setting setting, CancellationToken ct)
    {
        if (tenantId is null || tenantId != tenant.TenantId) return BadRequest();
        setting.TenantId = tenantId.Value;
        var updated = await service.EditSettingAsync(setting, ct);
        return updated is null ? StatusCode(417) : Ok(updated);
    }

    [HttpDelete("v1/setting/{settingId:long}")]
    [CheckAccessIfAdmin]
    public async Task<IActionResult> DeleteSetting(long settingId, CancellationToken ct)
        => await service.RemoveSettingAsync(settingId, ct) ? Ok() : StatusCode(417);

    [HttpGet("v1/setting/tenantName")]
    [CheckAccess]
    public IActionResult GetTenantName() => Ok(service.GetTenantName());
}
