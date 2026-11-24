using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Common.controller;
[ApiController,Route("api"),Authorize(Policy="SemRequestUriAccess")]
public sealed class CartableRoyalVisionController(ICartableRoyalVisionControllerService service):ControllerBase
{
    [HttpPost("v1/cartableRoyalVision/page")]
    public IActionResult GetTaskList([FromQuery]int page,[FromQuery]int size,[FromQuery]string? orderBy=null,[FromQuery]string? direction=null,[FromBody]string? body=null)
        =>Ok(service.GetTaskList(body,page,size,orderBy,direction));
    [HttpPost("v2/cartableRoyalVision/page")]
    public IActionResult GetTaskListV2([FromQuery]int page,[FromQuery]int size,[FromQuery]string? orderBy=null,[FromQuery]string? direction=null,[FromBody]string? body=null)
        =>Ok(service.GetTaskListV2(body,page,size,orderBy,direction));
    [HttpPost("v2/cartableRoyalVision/count")]
    public IActionResult GetTotalTaskCount([FromBody]string body)=>Ok(service.GetTotalTaskCount(body));
    [HttpPost("v1/cartableRoyalVision/list")]
    public IActionResult GetTaskListByFilter([FromQuery]string? orderBy=null,[FromQuery]string? direction=null,[FromBody]string? body=null)
        =>Ok(service.GetTaskListByFilter(body,orderBy,direction));
    [HttpPost("v1/cartableRoyalVisionCompleted/list")]
    public IActionResult GetTaskListByFilterCompleted([FromQuery]string? orderBy=null,[FromQuery]string? direction=null,[FromBody]string? body=null)
        =>Ok(service.GetTaskListByFilterCompleted(body,orderBy,direction));
    [HttpPost("v1/cartableRoyalVisionCompleted/page")]
    public IActionResult GetTaskListCompleted([FromQuery]int page,[FromQuery]int size,[FromQuery]string? orderBy=null,[FromQuery]string? direction=null,[FromBody]string? body=null)
        =>Ok(service.GetTaskListCompleted(body,page,size,orderBy,direction));
}
