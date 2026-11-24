using Common.controller;
using Common.exception;
using Common.model.dto;
using Common.model.entity;
using Common.repository;
using Common.utils;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;
using SortDirection = SemBackDotNet10.Common.enums.SortDirection;
namespace Common.service.impl;

public interface IAttachmentStorage
{
    // Host enforces authorization, size limits, malware policy, no symlink escapes and safe content types.
    void CreateUploadDirectory();
    void Write(string fileName,Stream content);
    AttachmentDownload Read(string fileName);
}
public interface IAttachmentOperationScope
{
    // Host coordinates DB transaction and filesystem compensation; failures must not leave orphan files.
    T Execute<T>(Func<T> operation);
}
public interface IAttachmentDateProvider
{
    // Preserve ICU fa_IR YYYYMMdd (week-year), application timezone and numeric formatting explicitly.
    string GetShamsiDate();
}
public sealed class AttachmentServiceImpl(AttachmentRepository repository,IAttachmentStorage storage,IAttachmentOperationScope operations,IAttachmentDateProvider date):Common.service.interf.AttachmentService
{
    private static readonly HashSet<string> Extensions=new(StringComparer.OrdinalIgnoreCase){"jpg","pdf","svg","png","jpeg","gif","doc","docx","xls","xlsx","ppt","pptx","txt","json","xml","rar","zip"};
    public AttachmentRepository GetRepository()=>repository;
    public object LoadGridById(string? body,int page,int size,string? orderBy,string? direction)
    {
        var json=JsonNode.Parse(body??throw new ArgumentNullException(nameof(body))) as JsonObject ?? throw new FormatException("Expected JSON object.");
        var entityId=json["entityId"] is null? (long?)null : long.Parse(json["entityId"]!.ToString(),System.Globalization.CultureInfo.InvariantCulture);
        return repository.LoadGridById(PageUtils.GetInstance(page,size,orderBy,ParseDirection(direction)),json["entityName"]?.GetValue<string>(),entityId);
    }
    internal static SortDirection? ParseDirection(string? value)=>value is null?null:
        value.Equals("ASC",StringComparison.OrdinalIgnoreCase)?SortDirection.Asc:
        value.Equals("DESC",StringComparison.OrdinalIgnoreCase)?SortDirection.Desc:throw new ArgumentException("Expected ASC or DESC.",nameof(value));
    public List<AttachmentDto> Create(List<IFormFile> files,string name,long id,string des)=>operations.Execute(()=>
    {
        var result=new List<AttachmentDto>();
        foreach(var file in files)
        {
            var ext=Path.GetExtension(file.FileName).TrimStart('.');
            if(!Extensions.Contains(ext))throw new BusinessException("فایل بارگذاری شده مجاز نیست.");
            var fileName=BuildFileName(name,id)+"."+ext;
            Upload(file,fileName);
            var saved=repository.Save(new Attachment{EntityName=name,EntityId=id,FileName=fileName,Des=des,UploadUrl="~/Upload/"+fileName});
            result.Add(new AttachmentDto{Id=saved.Id,FileName=saved.FileName,Des=saved.Des});
        }
        return result;
    });
    private string BuildFileName(string entityName,long entityId)
    {
        var last=repository.GetLastFileByEntityNameAndEntityId(entityName,entityId);
        long counter=1;
        if(last is not null)
        {
            var stem=Path.GetFileNameWithoutExtension(last.FileName??throw new FormatException("Missing stored filename."));
            counter=checked(long.Parse(stem[(stem.LastIndexOf('_')+1)..],System.Globalization.CultureInfo.InvariantCulture)+1);
        }
        var fileName=$"{entityName}_{entityId}_{date.GetShamsiDate()}_{counter}";
        ValidateFileName(fileName);return fileName;
    }
    public static void ValidateFileName(string name)
    {
        // Apply on reads AND writes: Java's download path was not confined to the storage root.
        if(string.IsNullOrWhiteSpace(name) || name is "." or ".." || name.IndexOfAny(['/', '\\', ':','\0'])>=0 || name.Any(char.IsControl) || Path.IsPathRooted(name))
            throw new BusinessException("error.attachment");
    }
    public void Upload(IFormFile file,string fileName)
    {
        ValidateFileName(fileName);
        if(file.Length==0)throw new BusinessException("error.attachment.fileIsEmpty");
        using var stream=file.OpenReadStream();storage.Write(fileName,stream);
    }
    public Attachment FindByIdOrElseThrow(long? id)=>id is null?throw new BusinessException("error.attachment.idIsNotValid"):
        repository.FindById(id)??throw new BusinessException("error.attachment.idIsNotValid");
    public void Delete(long id)=>operations.Execute(()=>{var entity=FindByIdOrElseThrow(id);entity.Deleted=true;repository.Save(entity);return true;});
    public AttachmentDownload LoadAsResource(string filename){ValidateFileName(filename);return storage.Read(filename);}
    // No raw path exposed: host resolves confined storage paths.
    public void CreateUploadDirectory()=>storage.CreateUploadDirectory();
    public string CreateJsonFile(JsonArray jsonArray,string entityName)=>operations.Execute(()=>
    {
        var stem=BuildFileName(entityName,1);using var stream=new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonArray.ToJsonString()));
        storage.Write(stem+".json",stream);return CreateAttachment(stem,entityName,1);
    });
    public string CreateAttachment(string fileName,string entityName,long entityId)
    {
        fileName+=".json";ValidateFileName(fileName);
        repository.Save(new Attachment{EntityName=entityName,EntityId=entityId,FileName=fileName,UploadUrl="~/Upload/"+fileName});return fileName;
    }
}
