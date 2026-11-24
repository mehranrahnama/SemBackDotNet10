using SemBackDotNet10.service.impl;
using SemBackDotNet10.Common;
using Microsoft.AspNetCore.Mvc;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;

namespace SemBackDotNet10.Controllers;

/// <summary>Port of MeasureUnitController (base route /api/v1).</summary>
[ApiController]
[Route("api/v1")]
public class MeasureUnitController : ControllerBase
{
    private readonly MeasureUnitServiceImpl _service;

    public MeasureUnitController(MeasureUnitServiceImpl service) => _service = service;

    [HttpPost("measureunit/list")]
    [CheckAccess]
    public async Task<IActionResult> GetAllMeasureUnit([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection direction = SortDirection.Asc,
        [FromQuery] string? filter = null)
        => Ok(await _service.FindAllAsync(filter, new ServicePageRequest(page, size, orderBy ?? "Id",
            direction == SortDirection.Desc ? SemBackDotNet10.service.impl.SortDirection.Descending : SemBackDotNet10.service.impl.SortDirection.Ascending)));

    [HttpPost("measureunit/list/forMenu")]
    [CheckAccess]
    public async Task<IActionResult> GetAllMeasureUnitForMenu([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection direction = SortDirection.Asc,
        [FromQuery] string? filter = null)
        => Ok(await _service.FindAllAsync(filter, new ServicePageRequest(page, size, orderBy ?? "Id",
            direction == SortDirection.Desc ? SemBackDotNet10.service.impl.SortDirection.Descending : SemBackDotNet10.service.impl.SortDirection.Ascending)));

    [HttpPost("measureunit")]
    [CheckAccess]
    public async Task<IActionResult> CreateMeasureUnit([FromBody] MeasureUnitDto dto)
        => Ok(await _service.MapToDtoAsync(await _service.CreateAsync(dto)));

    [HttpPut("measureunit")]
    [CheckAccess]
    public async Task<IActionResult> UpdateMeasureUnit([FromBody] MeasureUnitDto dto)
        => Ok(await _service.MapToDtoAsync(await _service.EditAsync(dto)));

    [HttpDelete("measureunit/{id:long}")]
    [CheckAccess]
    public async Task<IActionResult> DeleteMeasureUnit(long id)
        {
        try { await _service.DeleteByIdAsync(id); }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 547 })
        { return Conflict(new { error = "error.MeasureUnit.delete" }); }
        return Ok();
    }
}
