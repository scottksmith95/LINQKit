using System;
using System.Linq.Expressions;

namespace LinqKit
{
    /// <summary>
    /// Extensibility point: If you want to modify expanded queries before executing them
    /// set your own functionality to override empty QueryOptimizer
    /// </summary>
    public class LinqKitExtension
    {
        /// <summary>
        /// Place to optimize your queries. Example: Add a reference to Nuget package Linq.Expression.Optimizer 
        /// and in your program initializers set LinqKitExtension.QueryOptimizer = ExpressionOptimizer.visit;
        /// </summary>
        public static Func<Expression, Expression> QueryOptimizer = e => e;
    }
}
