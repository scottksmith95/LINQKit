using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace LinqKit
{
    /// <summary>
    /// Extensibility point: If you want to modify expanded queries before executing them
    /// set your own functionality to override default QueryOptimizer.
    /// </summary>
    public static class LinqKitExtension
    {
        /// <summary>
        /// Place to optimize your queries. Example: Add a reference to Nuget package Linq.Expression.Optimizer 
        /// and in your program initializers set LinqKitExtension.QueryOptimizer = ExpressionOptimizer.visit;
        /// </summary>
        [PublicAPI]
        public static Func<Expression, Expression> QueryOptimizer = e => e;
    }
}
