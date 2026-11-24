using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.measureDomain.controller;

[ApiController]
[Route("api/v1/measure/weights")]
public sealed class MeasureWeightsController(ServiceRuntime runtime, PlaningLevelUserServiceImpl levels) : ControllerBase
{
    public sealed record WeightInput(long Id, float? Weight);

    [HttpPut]
    [CheckAccess]
    public async Task<IActionResult> Update([FromBody] List<WeightInput?> changes, CancellationToken ct)
    {
        if (changes.Count == 0 || changes.Any(x => x is null || x.Id <= 0 || x.Weight is not float w || !float.IsFinite(w)))
            return BadRequest(new { message = "شناسه و وزن عددی هر شاخص الزامی است." });

        // Like assigning repeatedly to the same entity, the last value for an ID wins.
        var updates = changes.Select(x => x!).GroupBy(x => x.Id).Select(g => g.Last()).ToArray();
        await runtime.TransactionAsync(async () =>
        {
            var targetLevels = new Dictionary<long, long>();
            foreach (var entry in updates)
            {
                var levelIds = await runtime.Db.Database.SqlQuery<long>($"""
                    SELECT CAST(c.levelid AS bigint) AS Value
                    FROM dbo.tbldimmeasure m
                    JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
                    WHERE m.measureid = {entry.Id} AND m.isdeleted = 0
                    """).ToListAsync(ct);
                if (levelIds.Count != 1) throw new global::SemBackDotNet10.Common.exception.BusinessException("error.Measure.idIsNotValid");
                targetLevels.Add(entry.Id, levelIds[0]);
            }
            foreach (var levelId in targetLevels.Values.Distinct())
                await levels.HasAccessInLevelIdAsync(levelId, runtime.User.Id, ct);

            foreach (var entry in updates)
            {
                var updated = await runtime.Db.Database.ExecuteSqlInterpolatedAsync($"""
                    UPDATE m SET weight = {entry.Weight!.Value}
                    FROM dbo.tbldimmeasure m
                    JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
                    WHERE m.measureid = {entry.Id} AND m.isdeleted = 0 AND c.levelid = {targetLevels[entry.Id]}
                    """, ct);
                if (updated == 0) throw new global::SemBackDotNet10.Common.exception.BusinessException("error.Measure.idIsNotValid");
            }
        }, ct);
        return Ok();
    }
}
