#if EFCORE
using System;
using System.Linq.Expressions;
using System.Linq;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace LinqKit
{
    /// <summary>
    /// <seealso cref="Extensions"/>
    /// </summary>
    public static class ExtensionsEFCore
    {
        /// <summary>
        /// LinqKit: Returns wrapper that automatically expands expressions using a default QueryOptimizer.
        /// </summary>
        [PublicAPI]
        public static IQueryable<T> AsExpandableEFCore<T>(this IQueryable<T> query)
        {
            return AsExpandableEFCore(query, LinqKitExtension.QueryOptimizer);
        }

        /// <summary>
        /// LinqKit: Returns wrapper that automatically expands expressions using a custom QueryOptimizer.
        /// </summary>
        [PublicAPI]
        public static IQueryable<T> AsExpandableEFCore<T>(this IQueryable<T> query, Func<Expression, Expression> queryOptimizer)
        {
            return Extensions.AsExpandable(query, queryOptimizer);
//            if (query == null)
//            {
//                throw new ArgumentNullException(nameof(query));
//            }

//            if (queryOptimizer == null)
//            {
//                throw new ArgumentNullException(nameof(queryOptimizer));
//            }

//            if (query is ExpandableQuery<T>)
//            {
//                return query;
//            }

//#if !(NET35 || NOEF || NOASYNCPROVIDER)
//            return ExpandableQueryFactory<T>.Create(query, queryOptimizer);
//#else
//            return new ExpandableQuery<T>(query, queryOptimizer);
//#endif
        }
    }
}
#endif