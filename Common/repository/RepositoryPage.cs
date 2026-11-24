using Common.utils;
namespace Common.repository;
public sealed record RepositoryPage<T>(IReadOnlyList<T> Content,long TotalElements,int TotalPages,int CurrentPage,int PageSize);
public static class RepositoryPaging
{
    public static RepositoryPage<T> Slice<T>(IReadOnlyList<T> items,PageRequest request,int? reportedSize=null)
    {
        var content=items.Skip(request.Offset).Take(request.Size).ToList();
        return new(content,items.Count,(int)Math.Ceiling((double)items.Count/request.Size),request.Page,reportedSize??request.Size);
    }
}
