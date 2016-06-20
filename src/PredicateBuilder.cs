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
        /// <summary> Always true </summary>
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        /// <summary> Always false </summary>
		public static Expression<Func<T, bool>> False<T>() { return f => false; }

        /// <summary> OR </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2.Expand(), expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                 (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary> AND </summary>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2.Expand(), expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                 (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
}