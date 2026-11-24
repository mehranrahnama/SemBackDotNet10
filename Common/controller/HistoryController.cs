using System.Globalization;
using System.Text.Json;
using Common.model.entity;
using Common.service.interf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Common.controller;

[ApiController]
[Route("api/v1")]
[Authorize(Policy="SemRequestUriAccess")]
public sealed class HistoryController(HistoryService service):ControllerBase
{
    [HttpPost("History")]
    public IActionResult Save([FromBody] History history)
    {
        var saved=service.Save(history);
        return saved is null ? StatusCode(417) : Ok(saved);
    }
    [HttpPost("HistoryList")]
    public IActionResult GetHistory([FromBody] Dictionary<string,object?> values)
    {
        static string Required(Dictionary<string,object?> map,string key)
        {
            if(!map.TryGetValue(key,out var value) || value is null || value is JsonElement{ValueKind:JsonValueKind.Null})
                throw new ArgumentException($"Missing field: {key}",key);
            return value.ToString()!;
        }
        var rows=service.GetByParam(Required(values,"entityName"),long.Parse(Required(values,"entityId"),NumberStyles.Integer,CultureInfo.InvariantCulture));
        return rows.Count==0 ? NoContent() : Ok(rows.ToArray());
    }
}
