using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.service.impl;
using Dto = SemBackDotNet10.service.impl.PlanPeriodDto;
using ApiSort = SemBackDotNet10.Common.enums.SortDirection;
using System.Text.Json;

namespace SemBackDotNet10.Controllers;

[ApiController]
[Route("api/v1")]
public class PlanPeriodController(PlanPeriodServiceImpl service, PlanPeriodRepositoryAdapter repository) : ControllerBase
{
    [HttpPost("planPeriods/page")]
    [HttpPost("planPeriods/page/forMenu")]
    [CheckAccess]
    public async Task<IActionResult> GetPlanPeriodList([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, CancellationToken ct, [FromQuery] ApiSort direction = ApiSort.Asc,
        [FromBody] JsonElement? body = null)
    {
        var filter = body is null || body.Value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined
            ? null : body.Value.ValueKind == JsonValueKind.String ? body.Value.GetString() : body.Value.GetRawText();
        var result = await service.FindAllAsync(filter, page, size, orderBy ?? "statusType",
            direction == ApiSort.Desc ? SortDirection.Descending : SortDirection.Ascending, ct);
        return Ok(new SearchDto.Builder().content(result.Content.ToList()).totalElements(result.TotalElements)
            .totalPages(result.TotalPages).currentPage(page).pageSize(size).build());
    }

    [HttpPost("planPeriod")]
    [CheckAccess]
    public async Task<IActionResult> CreatePlanPeriod([FromBody] Dto dto, CancellationToken ct)
        => Ok(PlanPeriodServiceImpl.MapToDto(await service.CreateAsync(dto, ct)));

    [HttpPut("planPeriod")]
    [CheckAccess]
    public async Task<IActionResult> UpdatePlanPeriod([FromBody] Dto dto, CancellationToken ct)
        => Ok(PlanPeriodServiceImpl.MapToDto(await service.EditAsync(dto, ct)));

    [HttpDelete("planPeriod/{id:int}")]
    [CheckAccess]
    public async Task<IActionResult> DeletePlanPeriod(int id, CancellationToken ct)
    {
        try { await repository.DeleteAsync(id, ct); }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        { throw new global::SemBackDotNet10.Common.exception.BusinessException("error.planperiod.assigned"); }
        return Ok();
    }
}
