using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SemBackDotNet10.service.impl;
using SortDirection = SemBackDotNet10.Common.enums.SortDirection;
using ServiceSortDirection = SemBackDotNet10.service.impl.SortDirection;

namespace SemBackDotNet10.Controllers;

[ApiController]
[Route("api/v1")]
public class UserController(IServiceProvider services, UserDetailsReader details, UserPageReader pages) : ControllerBase
{
    private UserServiceImpl service => services.GetRequiredService<UserServiceImpl>();
    private static ServicePageRequest Page(int page, int size, string? orderBy, SortDirection? direction)
        => new(page, Math.Min(size, 200), direction is null || string.IsNullOrWhiteSpace(orderBy) ? "Id" : orderBy,
            direction == SortDirection.Desc && !string.IsNullOrWhiteSpace(orderBy) ? ServiceSortDirection.Descending : ServiceSortDirection.Ascending);

    [HttpPost("user/page/{levelId:long}")]
    [HttpPost("user/page/forMenu/{levelId:long}")]
    [CheckAccess]
    public async Task<IActionResult> GetAll(long levelId, [FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, CancellationToken ct,
        [FromBody] JsonElement? body = null)
    {
        if (page < 0 || size <= 0) return BadRequest("Invalid page or size.");
        string? filter = null;
        if (body is { ValueKind: JsonValueKind.Object } value && value.TryGetProperty("filter", out var property))
        {
            if (property.ValueKind is not (JsonValueKind.String or JsonValueKind.Null)) return BadRequest("Invalid filter.");
            filter = property.ValueKind == JsonValueKind.String ? property.GetString() : null;
        }
        else if (body is { ValueKind: JsonValueKind.String } stringValue) filter = stringValue.GetString();
        else if (body is { ValueKind: not (JsonValueKind.Null or JsonValueKind.Object) }) return BadRequest("Invalid request body.");
        return Ok(await pages.FindAllAsync(filter, levelId, Page(page, size, orderBy, direction), ct));
    }

    [HttpPut("user")]
    [CheckAccess]
    public async Task<IActionResult> Update([FromBody] UserDto dto, CancellationToken ct)
        => Ok(service.MapToDto(await service.EditAsync(dto, ct)));

    [HttpGet("user/CountActiveUsers/{planId:long}")]
    [CheckAccess]
    public async Task<IActionResult> CountActiveUsers(long planId, CancellationToken ct)
        => Ok(await service.CountActiveUsersAsync(planId, ct));

    [HttpGet("user/details/{accountName}")]
    [CheckAccess]
    public async Task<IActionResult> GetDetails(string accountName, CancellationToken ct)
    {
        var user = await details.GetAsync(accountName, ct);
        return user is null ? Forbid() : Ok(user);
    }

    [HttpPost("user/create")]
    [CheckAccess]
    public async Task<IActionResult> Create([FromBody] UserDto dto, CancellationToken ct)
        => Ok(await service.CreateUserAsync(dto, ct));

    [HttpDelete("user/{id:long}")]
    [CheckAccess]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return Ok();
    }

    [HttpPost("user/tashilgar")]
    [CheckAccess]
    public async Task<IActionResult> GetTashilgar([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, CancellationToken ct,
        [FromBody] string? filter = null)
        => Ok(await service.GetAllTashilgarAsync(filter, Page(page, size, orderBy, direction), ct));

    [HttpPost("user/modir")]
    [CheckAccess]
    public async Task<IActionResult> GetModir([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, CancellationToken ct,
        [FromBody] string? filter = null)
        => Ok(await service.GetAllModirAsync(filter, Page(page, size, orderBy, direction), ct));

    public sealed record UnlockRequest(long? UserId);

    [HttpPost("user/unlock")]
    [CheckAccess]
    public async Task<IActionResult> Unlock([FromBody] UnlockRequest request, CancellationToken ct)
    {
        if (request.UserId is not long id) return BadRequest("UserId is required.");
        await service.UnlockUserByUserIdAsync(id, ct);
        return Ok();
    }

    [HttpPost("synchronize/active")]
    [CheckAccess]
    public async Task<IActionResult> SynchronizeActiveUsers(CancellationToken ct)
    {
        await service.SynchronizeWithActiveDirectoryAsync(ct);
        return Ok();
    }
}
