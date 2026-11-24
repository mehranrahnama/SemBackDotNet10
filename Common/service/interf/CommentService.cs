using Common.model.entity;
using Common.repository;
namespace Common.service.interf;

// The host resolves actual audit users; this is not a replacement User entity.
public sealed record CommentAuthor(long Id,string? UserName,string? NameF,string? FamilyF);
public interface ICommentAuditContext
{
    long CurrentUserId { get; }
    CommentAuthor GetCreator(Comment comment);
}
public interface CommentService
{
    CommentRepository GetRepository();
    Comment Edit(IReadOnlyDictionary<string,object?> values);
    bool EditLock(long id);
    Comment? SaveComment(Comment comment);
    List<Comment> FindAllByEntityIdAndEntityName(long id,string name);
    List<Comment> GetCommentByParam(IReadOnlyDictionary<string,object?> values);
}
