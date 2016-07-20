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

        /// <summary> Always true </summary>
        [Obsolete]
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        /// <summary> Always false </summary>
        [Obsolete]
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        /// <summary> OR </summary>
        public static Expression<Func<T, bool>> Or<T>([NotNull] this Expression<Func<T, bool>> expr1, [NotNull] Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2.Expand(), expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary> OR </summary>
        public static Expression<Func<T, bool>> Or<T>([NotNull] this ExpressionStarter<T> starter, [NotNull] Expression<Func<T, bool>> expr2)
        {
            return (starter.Predicate == null) ? (starter.Predicate = expr2) : (starter.Predicate = starter.Predicate.Or(expr2));
        }

        /// <summary> AND </summary>
        public static Expression<Func<T, bool>> And<T>([NotNull] this Expression<Func<T, bool>> expr1, [NotNull] Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2.Expand(), expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary> OR </summary>
        public static Expression<Func<T, bool>> And<T>([NotNull] this ExpressionStarter<T> starter, [NotNull] Expression<Func<T, bool>> expr2)
        {
            return (starter.Predicate == null) ? (starter.Predicate = expr2) : (starter.Predicate = starter.Predicate.And(expr2));
        }
    }
}