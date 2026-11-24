using System;
using System.Linq;
using System.Linq.Expressions;
using SemBackDotNet10.Common.enums;

namespace Common.utils;

public sealed record PageRequest(int Page, int Size, string? OrderBy=null, SortDirection? Direction=null)
{
    public int Offset
    {
        get
        {
            if (Page < 0) throw new ArgumentOutOfRangeException(nameof(Page));
            if (Size < 1) throw new ArgumentOutOfRangeException(nameof(Size));
            return checked(Page * Size);
        }
    }
}
public static class PageUtils
{
    public const int MAX_PAGE_SIZE=200;

    public static IQueryable<T> Apply<T>(this IQueryable<T> query, PageRequest request)
    {
        if (request.OrderBy is { Length: > 0 } path && request.Direction is { } direction)
        {
            var parameter = Expression.Parameter(typeof(T), "item");
            Expression property = parameter;
            foreach (var name in path.Split('.'))
            {
                var type = property.Type;
                System.Reflection.PropertyInfo? member = null;
                while (type is not null && member is null)
                {
                    member = type.GetProperty(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.DeclaredOnly);
                    type = type.BaseType;
                }
                property = Expression.Property(property, member ?? throw new ArgumentException("Unknown sort property.", nameof(request)));
            }
            var selector = Expression.Lambda(property, parameter);
            var method = direction == SortDirection.Desc ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
            query = query.Provider.CreateQuery<T>(Expression.Call(typeof(Queryable), method,
                new[] { typeof(T), property.Type }, query.Expression, Expression.Quote(selector)));
        }
        return query.Skip(request.Offset).Take(request.Size);
    }
    public static int ValidatePagesSize(int size) => Math.Min(size, MAX_PAGE_SIZE);
    public static PageRequest GetInstance(int page=0, int size=MAX_PAGE_SIZE,
        string? orderBy=null, SortDirection? direction=null)
    {
        if(page<0) throw new ArgumentOutOfRangeException(nameof(page));
        size=ValidatePagesSize(size);
        if(size<1) throw new ArgumentOutOfRangeException(nameof(size));
        if(direction is not null && !Enum.IsDefined(direction.Value))
            throw new ArgumentOutOfRangeException(nameof(direction));
        var request = new PageRequest(page, size,
            direction is not null && !string.IsNullOrWhiteSpace(orderBy) ? orderBy : null, direction);
        _ = request.Offset;
        return request;
    }
}
