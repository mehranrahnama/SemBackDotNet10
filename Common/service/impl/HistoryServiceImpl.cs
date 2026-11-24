using Common.model.entity;
using Common.repository;
using Common.service.interf;
using Common.exception;
using Microsoft.Extensions.Logging;
namespace Common.service.impl;

public sealed class HistoryServiceImpl(HistoryRepository repository,CommentRepository comments,IHistoryAuditContext audit,ILogger<HistoryServiceImpl> logger):HistoryService
{
    public HistoryRepository GetRepository()=>repository;
    public HistoryRepository GetHistoryRepository()=>repository;
    public CommentRepository GetCommentRepository()=>comments;
    public History Save(History history)=>repository.Save(history);
    public List<History> GetByParam(string? entityName,long? entityId)
    {
        if(string.IsNullOrEmpty(entityName))throw new ArgumentException("نام موجودیت خالی می باشد",nameof(entityName));
        if(entityId is null)throw new ArgumentException("شناسه موجودیت خالی می باشد",nameof(entityId));
        try
        {
            var rows=repository.FindAllByEntityNameAndEntityIdOrderByCreatedDate(entityName,entityId.Value);
            foreach(var row in rows)
            {
                var creator=audit.GetCreator(row);
                // Java concatenation renders null names as the literal "null".
                row.CreatorDisplayName=$"{creator.NameF ?? "null"} {creator.FamilyF ?? "null"}";
                row.UserId=creator.Id;row.CreationDate=row.CreatedDate;
            }
            return rows;
        }
        catch(Exception error)
        {
            logger.LogError(error,"Error fetching history");
            throw new InvalidOperationException("Error in fetching data",error);
        }
    }
    public History FindByEntityAndIdAndOutComeAndStatus(long id,string simpleName)
        =>repository.FindTopByEntityIdAndEntityNameEqualsIgnoreCaseAndStatusAndOutComeOrderByCreatedDateDesc(id,simpleName,30,"confirm")
            ?? throw new BusinessException("تاریخچه ایی با این مشخصات پیدا نشد");
}
