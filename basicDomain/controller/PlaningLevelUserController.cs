using global::Common.utils;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.Controllers;

[ApiController]
[Route("api/v1")]
public class PlaningLevelUserController(PlaningLevelUserServiceImpl service) : ControllerBase
{
    [HttpPost("planing-level-user")]
    [CheckAccess]
    public async Task<IActionResult> CreateOrDeleteLevel(
        [FromBody] RequestInfoTwoParam<long?, List<long>> requestInfo, CancellationToken ct)
    {
        if (requestInfo.Param1 is not long userId || requestInfo.Param2 is not { } levelIds)
            return BadRequest();

        await service.CreateOrDeleteAsync(userId, levelIds, ct);
        return Ok();
    }

    [HttpPost("planingLevelUser/create")]
    [CheckAccess]
    public async Task<IActionResult> CreatePlaningLevelUser([FromBody] PlaningLevelUserDto dto, CancellationToken ct)
    {
        await service.CreateAsync(dto, ct);
        return Ok();
    }

    [HttpPut("planingLevelUser/edit")]
    [CheckAccess]
    public async Task<IActionResult> EditPlaningLevelUser([FromBody] PlaningLevelUserDto dto, CancellationToken ct)
    {
        await service.EditAsync(dto, ct);
        return Ok();
    }
}
