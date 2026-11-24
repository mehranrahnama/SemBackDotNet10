using Common.model.dto;
using Microsoft.AspNetCore.Http;
namespace Common.controller;
// Host adapters must call real migrated/domain services, never fabricated data.
public sealed record AttachmentDownload(Stream Content,string FileName,string ContentType);
public interface IAttachmentControllerService
{
    object LoadGridById(string? body,int page,int size,string? orderBy,string? direction);
    List<AttachmentDto> Create(List<IFormFile> files,string name,long id,string des);
    void Delete(long id);
    AttachmentDownload LoadAsResource(string filename);
}
public interface ICartableControllerService
{
    Task<SemBackDotNet10.service.impl.ServicePage<Common.model.entity.Cartable>> GetTaskListForSanganAsync(long? planId, bool? completed, string? filter, int page, int size, string? orderBy, string? direction, CancellationToken cancellationToken);
}
