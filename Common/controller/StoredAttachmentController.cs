using Common.controller.dto.request;
using Common.controller.dto.response;
using Common.enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace Common.controller;
[ApiController,Route("api/v1/files"),Authorize(Policy="SemRequestUriAccess")]
public sealed class StoredAttachmentController(IStoredAttachmentControllerService service):ControllerBase
{
    [HttpPost("upload"),Consumes("multipart/form-data")]
    public IActionResult CreateAttachment([FromForm]CreateAttachmentDto dto,[FromForm]IFormFile? file=null)
        =>StatusCode(201,service.CreateAttachment(dto,file));
    [HttpGet("download/entity/{entityName}/{entityId:long}")]
    public IActionResult GetFiles(EntityNames entityName,long entityId)=>Download(service.GetFiles(entityId,entityName));
    [HttpGet("detail/entity/{entityName}/{entityId}")]
    public IActionResult GetFilesDetail(EntityNames entityName,string entityId)=>Ok(service.GetFilesDetail(entityId,entityName));
    [HttpGet("download/file/{attachmentId:long}")]
    public IActionResult GetFile(long attachmentId)=>Download(service.GetFile(attachmentId));
    [HttpDelete("file/{attachmentId:long}")]
    public IActionResult DeleteFile(long attachmentId){service.DeleteById(attachmentId);return NoContent();}
    [HttpPut("file"),Consumes("multipart/form-data")]
    public IActionResult EditFile([FromForm]EditAttachmentDto dto,[FromForm]IFormFile? file=null)
    {service.Edit(dto,file);return Ok();}
    private IActionResult Download(DownloadFileResDto dto)
    {
        if(dto.Data is null || dto.FileName is null || dto.FileSize is null || dto.FileSize<0)
            throw new InvalidOperationException("Incomplete download service response.");
        Response.ContentLength=dto.FileSize.Value;
        return File(dto.Data,"application/octet-stream",dto.FileName);
    }
}
