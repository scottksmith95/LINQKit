using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;

namespace LinqKit
{
    /// <summary>Refer to http://www.albahari.com/nutshell/linqkit.html and
    /// http://tomasp.net/blog/linq-expand.aspx for more information.</summary>
    public static class Extensions
    {
        /// <summary> LinqKit: Returns wrapper that automatically expands expressions </summary>
        public static IQueryable<T> AsExpandable<T>(this IQueryable<T> query)
        {
            if (query is ExpandableQuery<T>) return query;
#if !(NET35 || NOEF)
            return ExpandableQueryFactory<T>.Create(query);
#else
            return new ExpandableQuery<T>(query);
#endif
        }

        /// <summary> LinqKit: Expands expression </summary>
        public static Expression<TDelegate> Expand<TDelegate>(this Expression<TDelegate> expr)
        {
            return (Expression<TDelegate>)new ExpressionExpander().Visit(expr);
        }

        /// <summary> LinqKit: Expands expression </summary>
		public static Expression Expand(this Expression expr)
        {
            return new ExpressionExpander().Visit(expr);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        public static TResult Invoke<TResult>(this Expression<Func<TResult>> expr)
        {
            return expr.Compile().Invoke();
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        public static TResult Invoke<T1, TResult>(this Expression<Func<T1, TResult>> expr, T1 arg1)
        {
            return expr.Compile().Invoke(arg1);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        public static TResult Invoke<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expr, T1 arg1, T2 arg2)
        {
            return expr.Compile().Invoke(arg1, arg2);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        public static TResult Invoke<T1, T2, T3, TResult>(
            this Expression<Func<T1, T2, T3, TResult>> expr, T1 arg1, T2 arg2, T3 arg3)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        public static TResult Invoke<T1, T2, T3, T4, TResult>(
            this Expression<Func<T1, T2, T3, T4, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4);
        }

#if !(NET35 || NET40)
        /// <summary> LinqKit: Compile and invoke </summary>
        public static TResult Invoke<T1, T2, T3, T4, T5, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
		public static TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
		public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
        }

#if !NOEF
        private static class ExpandableQueryFactory<T>
        {
            public static readonly Func<IQueryable<T>, ExpandableQuery<T>> Create;

            static ExpandableQueryFactory()
            {
                if (!typeof(T).GetTypeInfo().IsClass)
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
#endif
#endif
    }
}