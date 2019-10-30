#if EF
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
    public static class ExtensionsEF
    {
        /// <summary>
        /// LinqKit: Returns wrapper that automatically expands expressions using a default QueryOptimizer.
        /// </summary>
        [PublicAPI]
        public static IQueryable<T> AsExpandableEF<T>(this IQueryable<T> query)
        {
            return AsExpandableEF(query, LinqKitExtension.QueryOptimizer);
        }

        /// <summary>
        /// LinqKit: Returns wrapper that automatically expands expressions using a custom QueryOptimizer.
        /// </summary>
        [PublicAPI]
        public static IQueryable<T> AsExpandableEF<T>(this IQueryable<T> query, Func<Expression, Expression> queryOptimizer)
        {
            return Extensions.AsExpandable(query, queryOptimizer);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<TResult>(this Expression<Func<TResult>> expr)
        {
            return Extensions.Invoke(expr);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, TResult>(this Expression<Func<T1, TResult>> expr, T1 arg1)
        {
            return Extensions.Invoke(expr, arg1);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expr, T1 arg1, T2 arg2)
        {
            return Extensions.Invoke(expr, arg1, arg2);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, T2, T3, TResult>(
            this Expression<Func<T1, T2, T3, TResult>> expr, T1 arg1, T2 arg2, T3 arg3)
        {
            return Extensions.Invoke(expr, arg1, arg2, arg3);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, T2, T3, T4, TResult>(
            this Expression<Func<T1, T2, T3, T4, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return Extensions.Invoke(expr, arg1, arg2, arg3, arg4);
        }

#if !(NET35 || NET40)
        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, T2, T3, T4, T5, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return Extensions.Invoke(expr, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, T2, T3, T4, T5, T6, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6)
        {
            return Extensions.Invoke(expr, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, T2, T3, T4, T5, T6, T7, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7)
        {
            return Extensions.Invoke(expr, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8)
        {
            return Extensions.Invoke(expr, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return Extensions.Invoke(expr, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return Extensions.Invoke(expr, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            return Extensions.Invoke(expr, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            return Extensions.Invoke(expr, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            return Extensions.Invoke(expr, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            return Extensions.Invoke(expr, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            return Extensions.Invoke(expr, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        /// <summary>LinqKit: Compile and invoke</summary>
        [PublicAPI]
        public static TResult InvokeEF<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            return Extensions.Invoke(expr, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
        }
#endif
    }
}
#endif