#nullable enable
using System;
using System.Collections.Generic;

namespace SemBackDotNet10.basicDomain.Repositories;

/// <summary>
/// Explicit paging contract for the planning repositories.
/// Page is zero-based. This does not replace Common.utils.PagedResult.
/// </summary>
public sealed class PlaningPagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public long TotalElements { get; init; }
    public int Page { get; init; }
    public int Size { get; init; }
    public long TotalPages => Size > 0
        ? TotalElements / Size + (TotalElements % Size == 0 ? 0 : 1)
        : 0;
}

internal static class PlaningPaging
{
    public static int GetOffset(int page, int size)
    {
        if (page < 0)
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be zero or greater.");
        if (size <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Size must be greater than zero.");
        return checked(page * size);
    }
}
