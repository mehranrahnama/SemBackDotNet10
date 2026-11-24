using System.Globalization;
using System.Text.Json;
using Common.model.entity;
using Common.repository;
using Common.service.interf;
using Microsoft.Extensions.Logging;
namespace Common.service.impl;

public sealed class CommentServiceImpl(CommentRepository repository,ICommentAuditContext audit,ILogger<CommentServiceImpl> logger):CommentService
{
    public CommentRepository GetRepository()=>repository;
    private static string Required(IReadOnlyDictionary<string,object?> values,string key)
    {
        if(!values.TryGetValue(key,out var value) || value is null || value is JsonElement {ValueKind:JsonValueKind.Null})
            throw new ArgumentException($"Missing field: {key}",key);
        return value.ToString()!;
    }
    private static long Id(IReadOnlyDictionary<string,object?> values,string key)
        =>long.Parse(Required(values,key),NumberStyles.Integer,CultureInfo.InvariantCulture);
    public Comment Edit(IReadOnlyDictionary<string,object?> values)
    {
        var found=repository.FindById(Id(values,"id")) ?? throw new KeyNotFoundException("comment does not exist!!!");
        if(found.Lock)throw new InvalidOperationException("you have not privilege to edit this comment");
        found.CommentDes=Required(values,"commentDes");
        return repository.Save(found);
    }
    public bool EditLock(long id)
    {
        var found=repository.FindById(id) ?? throw new KeyNotFoundException("comment does not exist!!!");
        if(!found.Lock){found.Lock=true;repository.Save(found);}
        return true;
    }
    public Comment? SaveComment(Comment comment)
    {
        try{return repository.Save(comment);}
        catch(Exception error){logger.LogError(error,"Comment save failed");return null;}
    }
    public List<Comment> FindAllByEntityIdAndEntityName(long id,string name)
        =>repository.FindAllByEntityIdAndEntityName(id,name);
    public List<Comment> GetCommentByParam(IReadOnlyDictionary<string,object?> values)
    {
        var rows=repository.FindAllByEntityNameAndEntityIdOrderByCreatedDateDesc(Required(values,"entityName"),Id(values,"entityId"));
        foreach(var row in rows)
        {
            var creator=audit.GetCreator(row);
            row.CommentDate=row.CreatedDate;row.Creator=creator.UserName;
            row.CreatorName=creator.NameF;row.CreatorLastName=creator.FamilyF;
            // Preserve Java's read-side persistence/locking behavior explicitly.
            if(creator.Id!=audit.CurrentUserId){row.Lock=true;repository.Save(row);}
        }
        return rows;
    }
}
