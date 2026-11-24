#nullable enable
using System;

namespace SemBackDotNet10.Common.Pagination;

/// <summary>
/// Minimal paging request for translated repository contracts.
/// PageNumber is zero-based. This is not the full Spring Pageable API.
/// </summary>
public sealed class Pageable
{
    public int PageNumber { get; }
    public int PageSize { get; }
    public int Offset { get; }

    public Pageable(int pageNumber = 0, int pageSize = 20)
    {
        if (pageNumber < 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be zero or greater.");
        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");

        PageNumber = pageNumber;
        PageSize = pageSize;
        Offset = checked(pageNumber * pageSize);
    }
}
