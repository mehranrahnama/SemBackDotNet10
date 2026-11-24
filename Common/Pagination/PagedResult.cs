using SemBackDotNet10.baseDomain.repository;

namespace SemBackDotNet10.Common.Pagination;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public long TotalElements { get; init; }
    public int Page { get; init; }
    public int Size { get; init; }

    public int TotalPages => checked((int)new PageResult<T>(Items, TotalElements, Page, Size).TotalPages);
}
