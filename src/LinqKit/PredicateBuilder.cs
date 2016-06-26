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
        /// <summary>
        /// The Predicate Operator
        /// </summary>
        public enum PredicateOperator
        {
            /// <summary>
            /// The "Or"
            /// </summary>
            Or,

            /// <summary>
            /// The "And"
            /// </summary>
            And
        }

        /// <summary> Always true </summary>
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        /// <summary> Always false </summary>
		public static Expression<Func<T, bool>> False<T>() { return f => false; }

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

        /// <summary>
        /// Creates a Predicate with can be "And" (true) or "Or" (false).
        /// </summary>
        /// <typeparam name="T">The generic Type</typeparam>
        /// <param name="startOperator">The start PredicateOperator (can be "And" or "Or").</param>
        /// <returns>Expression{Func{T, bool}}</returns>
        public static Expression<Func<T, bool>> Create<T>(PredicateOperator startOperator = PredicateOperator.Or)
        {
            return f => startOperator == PredicateOperator.And;
        }

        /// <summary>
        /// Extends the specified source Predicate with another Predicate and the specified PredicateOperator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first">The source Predicate.</param>
        /// <param name="second">The second Predicate.</param>
        /// <param name="operator">The Operator (can be "And" or "Or").</param>
        /// <returns>Expression{Func{T, bool}}</returns>
        public static Expression<Func<T, bool>> Extend<T>([NotNull] this Expression<Func<T, bool>> first, [NotNull] Expression<Func<T, bool>> second, PredicateOperator @operator = PredicateOperator.Or)
        {
            return @operator == PredicateOperator.Or ? first.Or(second) : first.And(second);
        }
    }
}