using Common.model.dto;
using Common.controller.dto.request;
using Common.controller.dto.response;
using Common.enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace Common.controller;
public interface ICartableRoyalVisionControllerService
{
    object GetTaskList(string? body,int page,int size,string? orderBy,string? direction);
    object GetTaskListV2(string? body,int page,int size,string? orderBy,string? direction);
    object GetTotalTaskCount(string body);
    object GetTaskListByFilter(string? body,string? orderBy,string? direction);
    object GetTaskListByFilterCompleted(string? body,string? orderBy,string? direction);
    object GetTaskListCompleted(string? body,int page,int size,string? orderBy,string? direction);
}
public interface ICartableStatusControllerService
{
    void CreateCartableStatusLink(List<CartableStatusDto> items);
    List<CartableStatusDto> GetByEntityNameAndOperationTypeAndOutCome(CartableStatusDto dto);
    List<CartableStatusDto> GetByEntityNameAndOperationType(CartableStatusDto dto);
    List<CartableStatusDto> FindConfirmationLink(string? body);
}
public interface IReportControllerService
{
    string GenerateReport(string? filter,string fileFormat,string reportName);
    byte[] GenerateReportAsByteArray(string? filter,string fileFormat,string reportName);
}
public interface IReportFileTimestamp { string GetPersianTimestamp(); }
// Adapter must deserialize/render planingLevel.mrt with the licensed Stimulsoft runtime,
// supply reportPath/report/options and return the rendered 'index' view.
public interface IStimulsoftReportRenderer { Task<IActionResult> RenderIndex(CancellationToken cancellationToken); }
public interface IStoredAttachmentControllerService
{
    StoredAttachmentResDto CreateAttachment(CreateAttachmentDto dto,IFormFile? file);
    DownloadFileResDto GetFiles(long entityId,EntityNames entityName);
    List<StoredAttachmentResDto> GetFilesDetail(string entityId,EntityNames entityName);
    DownloadFileResDto GetFile(long attachmentId);
    void DeleteById(long attachmentId);
    void Edit(EditAttachmentDto dto,IFormFile? file);
}
