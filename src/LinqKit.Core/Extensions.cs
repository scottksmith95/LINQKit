using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using LinqKit.Utilities;

#if NOEF
namespace LinqKit.Core
#else
namespace LinqKit
#endif
{
    /// <summary>
    /// Refer to http://www.albahari.com/nutshell/linqkit.html and http://tomasp.net/blog/linq-expand.aspx for more information.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// LinqKit: Returns wrapper that automatically expands expressions using a default QueryOptimizer.
        /// </summary>
        [PublicAPI]
        [Pure]
        public static IQueryable<T> AsExpandable<T>(this IQueryable<T> query)
        {
            return AsExpandable(query, LinqKitExtension.QueryOptimizer);
        }

        /// <summary>
        /// LinqKit: Returns wrapper that automatically expands expressions using a custom QueryOptimizer.
        /// </summary>
        [PublicAPI]
        [Pure]
        public static IQueryable<T> AsExpandable<T>(this IQueryable<T> query, Func<Expression, Expression> queryOptimizer)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (queryOptimizer == null)
            {
                throw new ArgumentNullException(nameof(queryOptimizer));
            }

            if (query is ExpandableQuery<T>)
            {
                return query;
            }

#if !(NET35 || NOEF || NOASYNCPROVIDER)
            return ExpandableQueryFactory<T>.Create(query, queryOptimizer);
#else
            return new ExpandableQuery<T>(query, queryOptimizer);
#endif
        }

#if !(NET35 || NOEF || NOASYNCPROVIDER)
        private static class ExpandableQueryFactory<T>
        {
            public static readonly Func<IQueryable<T>, Func<Expression, Expression>, ExpandableQuery<T>> Create;

            static ExpandableQueryFactory()
            {
                if (!typeof(T).GetTypeInfo().IsClass)
                {
                    Create = (query, optimizer) => new ExpandableQuery<T>(query, optimizer);
                    return;
                }

                Type queryType = typeof(IQueryable<T>);
                Type optimizerType = typeof(Func<Expression, Expression>);

                var ctorInfo = typeof(ExpandableQueryOfClass<>).MakeGenericType(typeof(T)).GetConstructor(new[] { queryType, optimizerType });

                var queryParam = Expression.Parameter(queryType);
                var optimizerParam = Expression.Parameter(optimizerType);

                var newExpr = Expression.New(ctorInfo, queryParam, optimizerParam);
                var createExpr = Expression.Lambda<Func<IQueryable<T>, Func<Expression, Expression>, ExpandableQuery<T>>>(newExpr, queryParam, optimizerParam);

                Create = createExpr.Compile();
            }
        }
#endif
    }
}