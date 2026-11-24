using Common.controller;
using Common.model.entity;
using Common.model.dto;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;
namespace Common.service.interf;
// Raw physical Load(path) is intentionally not exposed; use confined LoadAsResource.
public interface AttachmentService : IAttachmentControllerService
{
    Attachment FindByIdOrElseThrow(long? id);
    void CreateUploadDirectory();
    string CreateJsonFile(JsonArray jsonArray,string entityName);
    string CreateAttachment(string fileName,string entityName,long entityId);
}
