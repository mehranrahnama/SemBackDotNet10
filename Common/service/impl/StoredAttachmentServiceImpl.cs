using System.Globalization;
using System.IO.Compression;
using Common.controller.dto.request;
using Common.controller.dto.response;
using Common.enums;
using Common.exception;
using Common.messages;
using Common.model.entity;
using Common.repository;
using Common.service.interf;
using Microsoft.AspNetCore.Http;
namespace Common.service.impl;
// Host must authorize every operation, enforce upload/decompression limits, supply audit
// and trusted absolute download URLs, and execute each scope in one shared DB transaction.
public interface IStoredAttachmentHost
{
    T Execute<T>(Func<T> action);
    StoredAttachmentResDto Map(StoredAttachment attachment);
}
public sealed class StoredAttachmentServiceImpl(StoredAttachmentRepository attachments,StoredFileRepository files,IStoredAttachmentHost host):StoredAttachmentService
{
    private T Scope<T>(Func<T> action)=>host.Execute(action);
    private StoredAttachment Require(long id)=>attachments.FindById(id)??throw new BusinessException(BusinessMessages.STORED_FILE_NOT_FOUND);
    public static void ValidateFileName(string? name)
    {
        if(name is not null && (name.Contains('/')||name.Contains('\\')||name.Contains("..")||name.Any(char.IsControl)||name.Contains(':')||name.EndsWith(".exe",StringComparison.OrdinalIgnoreCase)))
            throw new BusinessException(BusinessMessages.INVALID_FILE_TYPE);
    }
    private static StoredFile Read(IFormFile file)
    {
        if(string.IsNullOrEmpty(file.ContentType)) throw new BusinessException(BusinessMessages.INVALID_FILE_TYPE);
        ValidateFileName(file.FileName);
        using var data=new MemoryStream();file.CopyTo(data);return new StoredFile{Data=data.ToArray()};
    }
    private static void SetFile(StoredAttachment a,IFormFile file,StoredFile stored)
    {a.StoredFile=stored;a.FileName=file.FileName;a.FileType=file.ContentType;a.FileSize=file.Length;}
    public StoredAttachmentResDto CreateAttachment(CreateAttachmentDto dto,IFormFile? file)=>Scope(()=>{
        var resource=dto.EntityName??throw new ArgumentException("entityName required");
        var row=checked(attachments.FindMaxRowNoForUpdate(resource,dto.EntityId!)+1);
        StoredFile? stored=null;
        try{if(file is {Length:>0})stored=Read(file);}catch(IOException){throw new BusinessException(BusinessMessages.ATTACHMENT_FILE_READ_ERROR);}
        var a=new StoredAttachment{Resource=resource,EntityId=dto.EntityId,Description=dto.Description,RowNo=row};
        if(stored is not null)SetFile(a,file!,stored);
        try{if(stored is not null)files.Save(stored);return host.Map(attachments.Save(a));}
        catch(Exception){throw new BusinessException(BusinessMessages.ATTACHMENT_FILE_SAVE_ERROR);}
    });
    public DownloadFileResDto GetFile(long id)=>Scope(()=>{
        var a=Require(id);if(a.StoredFile?.Data is not byte[] data)throw new BusinessException(BusinessMessages.DATA_INCONSISTENT);
        ValidateFileName(a.FileName);
        return new DownloadFileResDto{FileName=a.FileName,Data=new MemoryStream(data,false),FileSize=a.FileSize};
    });
    public DownloadFileResDto GetFiles(long entityId,EntityNames resource)=>Scope(()=>{
        var values=attachments.FindByEntityIdAndResource(entityId,resource);
        if(values.Count==0)throw new BusinessException(BusinessMessages.STORED_FILE_NOT_FOUND);
        try{
            using var output=new MemoryStream();
            using(var zip=new ZipArchive(output,ZipArchiveMode.Create,true))foreach(var a in values){
                if(a.StoredFile?.Data is not byte[] data)continue;
                ValidateFileName(a.FileName);var entry=zip.CreateEntry($"{a.Id}_{a.FileName??"file"}");
                using var stream=entry.Open();stream.Write(data);
            }
            // Java also returns a non-empty empty-ZIP container when every file is absent.
            var bytes=output.ToArray();return new DownloadFileResDto{FileName="attached-files.zip",Data=new MemoryStream(bytes,false),FileSize=bytes.LongLength};
        }catch(IOException){throw new BusinessException(BusinessMessages.ATTACHMENT_FILE_DOWNLOAD_ERROR);}
    });
    public List<StoredAttachmentResDto> GetFilesDetail(string entityId,EntityNames resource)=>Scope(()=>{
        try{return attachments.FindAllFilesByResourceAndEntityId(resource,entityId).Select(host.Map).ToList();}
        catch(Exception){throw new BusinessException(BusinessMessages.DATA_INCONSISTENT);}
    });
    public void DeleteById(long id)=>Scope(()=>{
        try{var a=Require(id);attachments.DeleteById(id);if(a.StoredFile is not null)files.DeleteById(a.StoredFile.FileId);return true;}
        catch(BusinessException){throw;}catch(Exception){throw new BusinessException(BusinessMessages.DATA_INCONSISTENT);}
    });
    public bool HasUploadedFile(EntityNames resource,string id)
    {try{return Scope(()=>attachments.ExistsUploadedFileByResourceAndEntityId(resource,id));}catch(Exception){return false;}}
    public void Edit(EditAttachmentDto dto,IFormFile? file)=>Scope(()=>{
        var a=Require(dto.Id??throw new ArgumentException("id required"));
        try{
            a.Description=dto.Description;var old=a.StoredFile;
            if(dto.RemoveFile==true){a.StoredFile=null;a.FileName=null;a.FileType=null;a.FileSize=null;attachments.Save(a);if(old is not null)files.Delete(old);}
            else if(file is {Length:>0}){var stored=Read(file);files.Save(stored);SetFile(a,file,stored);attachments.Save(a);if(old is not null)files.Delete(old);}
            else attachments.Save(a);
            return true;
        }catch(IOException){throw new BusinessException(BusinessMessages.ATTACHMENT_FILE_READ_ERROR);}
        catch(BusinessException){throw;}catch(Exception){throw new BusinessException(BusinessMessages.ATTACHMENT_FILE_SAVE_ERROR);}
    });
    public void DeleteByResourceAndEntityIds(EntityNames resource,List<long> ids)
    {
        if(ids is null||ids.Count==0)return;
        Scope(()=>{var strings=ids.Select(x=>x.ToString(CultureInfo.InvariantCulture)).ToList();
            var fileIds=attachments.FindFileIdsByResourceAndEntityIdIn(resource,strings);
            attachments.DeleteByResourceAndEntityIdIn(resource,strings);foreach(var id in fileIds)files.DeleteById(id);return true;});
    }
    public HashSet<long> FindEntityIdsWithUploadedFile(EntityNames resource,List<long> ids)
    {
        if(ids is null||ids.Count==0)return [];
        return Scope(()=>attachments.FindEntityIdsWithFile(resource,ids.Select(x=>x.ToString(CultureInfo.InvariantCulture)).ToList()).Select(x=>long.Parse(x,CultureInfo.InvariantCulture)).ToHashSet());
    }
}
