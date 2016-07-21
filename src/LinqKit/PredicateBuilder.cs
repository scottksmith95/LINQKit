using JetBrains.Annotations;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace LinqKit
{
    /// <summary>
    /// See http://www.albahari.com/expressions for information and examples.
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary> Start an expression </summary>
        public static ExpressionStarter<T> New<T>(Expression<Func<T, bool>> expr = null) { return new ExpressionStarter<T>(expr); }

        /// <summary> Create an expression with a stub expression true or false to use when the expression is not yet started. </summary>
        public static ExpressionStarter<T> New<T>(bool defaultExpression) { return new ExpressionStarter<T>(defaultExpression); }

        /// <summary> Always true </summary>
        [Obsolete("Use PredicateBuilder.New() instead.")]
        public static Expression<Func<T, bool>> True<T>() { return new ExpressionStarter<T>(true); }

        /// <summary> Always false </summary>
        [Obsolete("Use PredicateBuilder.New() instead.")]
        public static Expression<Func<T, bool>> False<T>() { return new ExpressionStarter<T>(false); }

        /// <summary> OR </summary>
        public static Expression<Func<T, bool>> Or<T>([NotNull] this Expression<Func<T, bool>> expr1, [NotNull] Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2.Expand(), expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary> AND </summary>
        public static Expression<Func<T, bool>> And<T>([NotNull] this Expression<Func<T, bool>> expr1, [NotNull] Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2.Expand(), expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
}