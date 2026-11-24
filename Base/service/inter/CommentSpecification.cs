using System;
using System.Linq.Expressions;

using SemBackDotNet10.Base.Model.Entity;

using SemBackDotNet10.Base.service.inter;

namespace SemBackDotNet10.Base.Service.Inter;

/// <summary>
/// معادل CommentSpecification.java — Specification جاوا (CriteriaBuilder.equal)
/// به predicate عبارت لامبدا (Expression&lt;Func&lt;T,bool&gt;&gt;) ترجمه شده که
/// ISemRepository.FindAllAsync(predicate) مصرف می‌کند.
/// root.get(criteria.getKey()) → reflection روی property با همان نام key.
/// </summary>
public class CommentSpecification<T> where T : class
{
    private readonly SearchCriteria criteria;

    public CommentSpecification(SearchCriteria searchCriteria)
    {
        criteria = searchCriteria;
    }

    /// <summary>معادل toPredicate(Root, CriteriaQuery, CriteriaBuilder) — builder.equal(root.get(key), value)</summary>
    public Expression<Func<T, bool>> ToPredicate()
    {
        var value = criteria.Value?.ToString();
        var parameter = Expression.Parameter(typeof(T), "e");
        var property = Expression.Property(parameter, criteria.Key);
        Expression left = property.Type == typeof(string)
            ? property
            : Expression.Call(property, "ToString", Type.EmptyTypes);
        var body = Expression.Equal(left, Expression.Constant(value));
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}
