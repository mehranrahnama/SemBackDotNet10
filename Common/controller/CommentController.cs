using Common.model.entity;
using Common.service.interf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Common.controller;

[ApiController]
[Route("api")]
[Authorize(Policy="SemRequestUriAccess")]
public sealed class CommentController(CommentService service):ControllerBase
{
    [HttpPost("v1/comment")]
    public IActionResult Save([FromBody] Comment comment)
    {
        var saved=service.SaveComment(comment);
        return saved is null?StatusCode(417):Ok(saved);
    }
    [HttpPut("v1/comment")]
    public IActionResult Edit([FromBody] Dictionary<string,object?> values)=>Ok(service.Edit(values));
    [HttpPut("v1/lock/{id:long}")]
    public IActionResult EditStatus(long id)=>service.EditLock(id)?Ok():StatusCode(417);
    [HttpPost("v1/allComment")]
    public IActionResult Get([FromBody] Dictionary<string,object?> values)
    {
        var rows=service.GetCommentByParam(values);
        return rows.Count==0?NoContent():Ok(rows);
    }
}
