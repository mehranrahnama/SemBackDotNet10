using Common.model.entity;
using Common.repository;
namespace Common.service.interf;

public sealed record HistoryAuthor(long Id,string? NameF,string? FamilyF);
public interface IHistoryAuditContext { HistoryAuthor GetCreator(History history); }
public interface HistoryService
{
    HistoryRepository GetRepository();
    HistoryRepository GetHistoryRepository();
    CommentRepository GetCommentRepository();
    History Save(History history);
    List<History> GetByParam(string? entityName,long? entityId);
    History FindByEntityAndIdAndOutComeAndStatus(long id,string simpleName);
}
