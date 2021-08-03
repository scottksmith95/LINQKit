﻿using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqKit
{
    /// <summary> The Predicate Operator </summary>
    public enum PredicateOperator
    {
        /// <summary> The "Or" </summary>
        Or,

        /// <summary> The "And" </summary>
        And
    }

    /// <summary>
    /// See http://www.albahari.com/expressions for information and examples.
    /// </summary>
    public static class PredicateBuilder
    {
        private class RebindParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            public RebindParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node == _oldParameter)
                {
                    return _newParameter;
                }

                return base.VisitParameter(node);
            }
        }

        /// <summary> Start an expression </summary>
        public static ExpressionStarter<T> New<T>() { return new ExpressionStarter<T>(); }
        
        /// <summary> Start an expression </summary>
        /// <param name="expression">Expression to be used when starting the builder.</param>
        public static ExpressionStarter<T> New<T>(Expression<Func<T, bool>> expression) { return new ExpressionStarter<T>(expression); }
        
        /// <summary> Create an expression with a stub expression true or false to use when the expression is not yet started. </summary>
        public static ExpressionStarter<T> New<T>(bool defaultExpression) { return new ExpressionStarter<T>(defaultExpression); }

        /// <summary>
        /// Create an expression using an <see cref="IEnumerable{T}"/>.
        /// May be used to instantiate ExpressionStarter objects with anonymous types.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="enumerable">The value is NOT used. Only serves as a way to provide the generic type.</param>
        public static ExpressionStarter<T> New<T>(IEnumerable<T> enumerable)
            => New<T>();
        
        /// <summary>
        /// Create an expression using an <see cref="IEnumerable{T}"/>.
        /// May be used to instantiate ExpressionStarter objects with anonymous types.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="enumerable">The value is NOT used. Only serves as a way to provide the generic type.</param>
        /// <param name="expression">Expression to be used when starting the builder.</param>
        public static ExpressionStarter<T> New<T>(IEnumerable<T> enumerable, Expression<Func<T, bool>> expression)
            => New(expression);

        /// <summary>
        /// Create an expression using an <see cref="IEnumerable{T}"/> with default starting state.
        /// May be used to instantiate ExpressionStarter objects with anonymous types.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="enumerable">The value is NOT used. Only serves as a way to provide the generic type.</param>
        /// <param name="state">Boolean state used when there is not starting expression yet.</param>
        public static ExpressionStarter<T> New<T>(IEnumerable<T> enumerable, bool state)
            => New<T>(state);

        /// <summary> Always true </summary>
        [Obsolete("Use PredicateBuilder.New() instead.")]
        public static Expression<Func<T, bool>> True<T>() { return new ExpressionStarter<T>(true); }

        /// <summary> Always false </summary>
        [Obsolete("Use PredicateBuilder.New() instead.")]
        public static Expression<Func<T, bool>> False<T>() { return new ExpressionStarter<T>(false); }

        /// <summary> OR </summary>
        public static Expression<Func<T, bool>> Or<T>([NotNull] this Expression<Func<T, bool>> expr1, [NotNull] Expression<Func<T, bool>> expr2)
        {
            var expr2Body = new RebindParameterVisitor(expr2.Parameters[0], expr1.Parameters[0]).Visit(expr2.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, expr2Body), expr1.Parameters);
        }

        /// <summary> AND </summary>
        public static Expression<Func<T, bool>> And<T>([NotNull] this Expression<Func<T, bool>> expr1, [NotNull] Expression<Func<T, bool>> expr2)
        {
            var expr2Body = new RebindParameterVisitor(expr2.Parameters[0], expr1.Parameters[0]).Visit(expr2.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, expr2Body), expr1.Parameters);
        }

        /// <summary> NOT </summary>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
            => Expression.Lambda<Func<T, bool>>(Expression.Not(expr.Body), expr.Parameters);

        /// <summary>
        /// Extends the specified source Predicate with another Predicate and the specified PredicateOperator.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="first">The source Predicate.</param>
        /// <param name="second">The second Predicate.</param>
        /// <param name="operator">The Operator (can be "And" or "Or").</param>
        /// <returns>Expression{Func{T, bool}}</returns>
        public static Expression<Func<T, bool>> Extend<T>([NotNull] this Expression<Func<T, bool>> first, [NotNull] Expression<Func<T, bool>> second, PredicateOperator @operator = PredicateOperator.Or)
        {
            return @operator == PredicateOperator.Or ? first.Or(second) : first.And(second);
        }

        /// <summary>
        /// Extends the specified source Predicate with another Predicate and the specified PredicateOperator.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="first">The source Predicate.</param>
        /// <param name="second">The second Predicate.</param>
        /// <param name="operator">The Operator (can be "And" or "Or").</param>
        /// <returns>Expression{Func{T, bool}}</returns>
        public static Expression<Func<T, bool>> Extend<T>([NotNull] this ExpressionStarter<T> first, [NotNull] Expression<Func<T, bool>> second, PredicateOperator @operator = PredicateOperator.Or)
        {
            return @operator == PredicateOperator.Or ? first.Or(second) : first.And(second);
        }
    }
}