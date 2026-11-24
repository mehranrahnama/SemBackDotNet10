using System.Linq.Expressions;
namespace Common.utils;
public static class SpecificationUtils
{
    // Does not mutate the caller's list, unlike the Java implementation.
    // Parameter replacement avoids Invoke expressions unsupported by many SQL providers.
    public static Expression<Func<T,bool>>? AndSpecifications<T>(IEnumerable<Expression<Func<T,bool>>>? specifications)
    {
        if (specifications is null) return null;
        var parameter=Expression.Parameter(typeof(T), "entity");
        Expression? body=null;
        foreach(var specification in specifications)
        {
            ArgumentNullException.ThrowIfNull(specification);
            var next=new ReplaceParameter(specification.Parameters[0],parameter).Visit(specification.Body)!;
            body=body is null ? next : Expression.AndAlso(body,next);
        }
        return body is null ? null : Expression.Lambda<Func<T,bool>>(body,parameter);
    }
    private sealed class ReplaceParameter(ParameterExpression from, ParameterExpression to) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) => node==from ? to : base.VisitParameter(node);
    }
}
