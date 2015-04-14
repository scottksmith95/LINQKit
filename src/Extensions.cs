using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace LinqKit
{
	/// <summary>Refer to http://www.albahari.com/nutshell/linqkit.html and
	/// http://tomasp.net/blog/linq-expand.aspx for more information.</summary>
	public static class Extensions
	{
        public static IQueryable<T> AsExpandable<T>(this IQueryable<T> query)
        {
            if (query is ExpandableQuery<T>) return query;
            return ExpandableQueryFactory<T>.Create(query);
        }

	    public static Expression<TDelegate> Expand<TDelegate>(this Expression<TDelegate> expr)
		{
			return (Expression<TDelegate>)new ExpressionExpander ().Visit (expr);
		}

		public static Expression Expand (this Expression expr)
		{
			return new ExpressionExpander ().Visit (expr);
		}

		public static TResult Invoke<TResult> (this Expression<Func<TResult>> expr)
		{
			return expr.Compile ().Invoke ();
		}

		public static TResult Invoke<T1, TResult> (this Expression<Func<T1, TResult>> expr, T1 arg1)
		{
			return expr.Compile ().Invoke (arg1);
		}

		public static TResult Invoke<T1, T2, TResult> (this Expression<Func<T1, T2, TResult>> expr, T1 arg1, T2 arg2)
		{
			return expr.Compile ().Invoke (arg1, arg2);
		}

		public static TResult Invoke<T1, T2, T3, TResult> (
			this Expression<Func<T1, T2, T3, TResult>> expr, T1 arg1, T2 arg2, T3 arg3)
		{
			return expr.Compile ().Invoke (arg1, arg2, arg3);
		}

		public static TResult Invoke<T1, T2, T3, T4, TResult> (
			this Expression<Func<T1, T2, T3, T4, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			return expr.Compile ().Invoke (arg1, arg2, arg3, arg4);
		}

		public static void ForEach<T> (this IEnumerable<T> source, Action<T> action)
		{
			foreach (var element in source)
				action (element);
		}

        private static class ExpandableQueryFactory<T>
        {
            public static readonly Func<IQueryable<T>, ExpandableQuery<T>> Create;

            static ExpandableQueryFactory()
            {
                if (!typeof(T).IsClass)
                {
                    Create = query => new ExpandableQuery<T>(query);
                    return;
                }

                var queryType = typeof(IQueryable<T>);
                var ctorInfo = typeof(ExpandableQueryOfClass<>).MakeGenericType(typeof(T)).GetConstructor(new[] { queryType });
                var queryParam = Expression.Parameter(queryType);
                var newExpr = Expression.New(ctorInfo, queryParam);
                var createExpr = Expression.Lambda<Func<IQueryable<T>, ExpandableQuery<T>>>(newExpr, queryParam);
                Create = createExpr.Compile();
            }
        }
    }
}