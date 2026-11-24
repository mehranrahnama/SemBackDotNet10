using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using Common.exception;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace Common.exception.handler;
public interface IExceptionResponseContext
{
    string GetMessage(string code,params object?[] args);
    string? ServerAddress { get; }
    string? CurrentUser { get; }
    // Delete with the actual cookie Path/Domain used by the host.
    void DeleteSessionKey(HttpContext context);
}
public sealed class CustomizedExceptionHandler(RequestDelegate next,ILogger<CustomizedExceptionHandler> logger)
{
    public async Task InvokeAsync(HttpContext context,IExceptionResponseContext messages)
    {
        try {await next(context);}
        catch(Exception ex) when(!context.Response.HasStarted && !(ex is OperationCanceledException && context.RequestAborted.IsCancellationRequested))
        {
            logger.LogError(ex,"Unhandled request exception at {Path}",context.Request.Path);
            var (status,payload)=Map(ex,context,messages);
            context.Response.Clear();
            context.Response.StatusCode=status;
            if(ex is TokenRefreshException && context.Request.Cookies.ContainsKey("sessionKey")) messages.DeleteSessionKey(context);
            await context.Response.WriteAsJsonAsync(payload,cancellationToken:context.RequestAborted);
        }
    }
    public static (int Status,object Payload) Map(Exception ex,HttpContext context,IExceptionResponseContext messages)
    {
        ErrorDetails Details(string text,string? user=null,object? data=null)=>new([text],DateTime.Now,
            "uri="+context.Request.Path,user,messages.ServerAddress,data,errorCode:null);
        return ex switch
        {
            BusinessException e=>(417,Details(messages.GetMessage(e.Message,e.Args ?? []),data:e.Data)),
            InvalidStateException e=>(422,Details(e.Message)),
            ServiceException e=>(417,Details(messages.GetMessage(e.Message))),
            AccessForbiddenException e=>(403,new ErrorResponse(e.Message)),
            UnAuthorizedException=>(401,new ErrorResponse("عدم مجوز دسترسی")),
            TokenRefreshException e=>(401,new ErrorResponse(e.Message)),
            BruteForceException e=>(423,new ErrorDetails([e.Message],e.Params)),
            ValidationException e=>(400,new ErrorResponse("Data Validation Failed",[e.Message])),
            DbException=>(417,Details(messages.GetMessage("خطای پایگاه داده"),messages.CurrentUser)),
            _=>(500,Details(messages.GetMessage("خطای زمان اجرا"),messages.CurrentUser))
        };
    }
    // Call services.AddControllers().ConfigureApiBehaviorOptions(ConfigureValidation).
    public static void ConfigureValidation(ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory=context=>new BadRequestObjectResult(new ErrorResponse(
            "Data Validation Failed",context.ModelState.SelectMany(pair=>pair.Value!.Errors.Select(error=>
                pair.Key+": "+(string.IsNullOrEmpty(error.ErrorMessage)?"Invalid value":error.ErrorMessage))).ToList()));
    }
}
