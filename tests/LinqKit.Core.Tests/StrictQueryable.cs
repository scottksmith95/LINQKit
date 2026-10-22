using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqKitCoreTests;

/// <summary>
/// An in-memory IQueryable that, like a database provider, refuses to run an expression tree that still
/// contains LinqKit Invoke calls, LambdaExpression.Compile calls, InvocationExpressions, references to
/// expression variables or expressions stored as constants. LINQ to Objects would run those as delegates, so without this check a test could
/// pass even when AsExpandable or Expand did nothing.
/// </summary>
public static class StrictQueryable
{
    public static StrictQueryable<T> AsStrictQueryable<T>(this IEnumerable<T> source)
    {
        var query = source.AsQueryable();
        return new(query, new StrictQueryProvider(query.Provider));
    }
}

public sealed class StrictQueryProvider : IQueryProvider
{
    private readonly IQueryProvider _inner;

    internal StrictQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    /// <summary>The expression the provider was last asked to run.</summary>
    public Expression LastExecuted { get; private set; }

    public IQueryable CreateQuery(Expression expression)
    {
        var query = _inner.CreateQuery(expression);
        return (IQueryable)Activator.CreateInstance(
            typeof(StrictQueryable<>).MakeGenericType(query.ElementType),
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            [query, this],
            null);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
        new StrictQueryable<TElement>(_inner.CreateQuery<TElement>(expression), this);

    public object Execute(Expression expression) => _inner.Execute(Check(expression));

    public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(Check(expression));

    internal Expression Check(Expression expression)
    {
        LastExecuted = expression;
        new UntranslatableNodeFinder().Visit(expression);
        return expression;
    }

    private sealed class UntranslatableNodeFinder : ExpressionVisitor
    {
        protected override Expression VisitInvocation(InvocationExpression node) =>
            throw new NotSupportedException($"InvocationExpression is not translatable: {node}");

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == nameof(LambdaExpression.Compile) && typeof(LambdaExpression).IsAssignableFrom(node.Method.DeclaringType))
            {
                throw new NotSupportedException($"Compile() is not translatable: {node}");
            }

            if (node.Method.Name == "Invoke" && node.Method.DeclaringType?.Namespace?.StartsWith("LinqKit") == true)
            {
                throw new NotSupportedException($"LinqKit Invoke() is not translatable: {node}");
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is LambdaExpression)
            {
                throw new NotSupportedException($"An expression stored as a constant is not translatable: {node}");
            }

            return base.VisitConstant(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (typeof(LambdaExpression).IsAssignableFrom(node.Type))
            {
                throw new NotSupportedException($"A reference to an expression variable is not translatable: {node}");
            }

            return base.VisitMember(node);
        }
    }
}

public sealed class StrictQueryable<T> : IOrderedQueryable<T>
{
    private readonly IQueryable<T> _inner;
    private readonly StrictQueryProvider _provider;

    internal StrictQueryable(IQueryable<T> inner, StrictQueryProvider provider)
    {
        _inner = inner;
        _provider = provider;
    }

    public StrictQueryProvider StrictProvider => _provider;

    public Type ElementType => typeof(T);

    public Expression Expression => _inner.Expression;

    public IQueryProvider Provider => _provider;

    public IEnumerator<T> GetEnumerator()
    {
        _provider.Check(_inner.Expression);
        return _inner.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
