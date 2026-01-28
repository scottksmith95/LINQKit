#if NOEF
namespace LinqKit
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using JetBrains.Annotations;

    /// <summary>
    /// Refer to http://www.albahari.com/nutshell/linqkit.html and http://tomasp.net/blog/linq-expand.aspx for more information.
    /// </summary>
    public static partial class ExtensionsCore
    {
        /// <summary> LinqKit: Expands expression </summary>
        [Pure]
        public static Expression<TDelegate> Expand<TDelegate>(this Expression<TDelegate> expr)
        {
            return (Expression<TDelegate>)new ExpressionExpander().Visit(expr);
        }

        /// <summary> LinqKit: Expands expression </summary>
        [Pure]
        public static Expression Expand<TDelegate>(this ExpressionStarter<TDelegate> expr)
        {
            return expr != null && (expr.IsStarted || expr.UseDefaultExpression) ? new ExpressionExpander().Visit(expr) : null;
        }

        /// <summary> LinqKit: Expands expression </summary>
        [Pure]
        public static Expression Expand(this Expression expr)
        {
            return new ExpressionExpander().Visit(expr);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<TResult>(this Expression<Func<TResult>> expr)
        {
            return expr.Compile().Invoke();
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, TResult>(this Expression<Func<T1, TResult>> expr, T1 arg1)
        {
            return expr.Compile().Invoke(arg1);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expr, T1 arg1, T2 arg2)
        {
            return expr.Compile().Invoke(arg1, arg2);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, T2, T3, TResult>(
            this Expression<Func<T1, T2, T3, TResult>> expr, T1 arg1, T2 arg2, T3 arg3)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, TResult>(
            this Expression<Func<T1, T2, T3, T4, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Performs the specified action on each element of the IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the source.</typeparam>
        /// <param name="source">The IEnumerable&lt;T&gt; to perform the action on.</param>
        /// <param name="action">The Action&lt;T&gt; delegate to perform on each element of the IEnumerable&lt;T&gt;.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var element in source)
            {
                action(element);
            }
        }

#if !(NET35 || NET40)
        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, T5, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        /// <summary> LinqKit: Compile and invoke </summary>
        [Pure]
        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
        T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
        }
#endif
    }
}
#endif

#if !NET10_0_OR_GREATER
namespace LinqKit
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using JetBrains.Annotations;

    public static partial class ExtensionsCore
    {
        /// <summary>
        /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to left join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <returns>An System.Linq.IQueryable&lt;TResult&gt; that has elements of type TResult obtained by performing an inner join on two sequences.</returns>
        [Pure]
        public static IQueryable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer,
            IEnumerable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            var leftJoinedQueryable = outer
                .GroupJoin(inner, outerKeySelector, innerKeySelector, (outerItem, innerItems) => new { Outer = outerItem, IEnumerableInner = innerItems })
                .SelectMany(grouping => grouping.IEnumerableInner.DefaultIfEmpty(), (grouping, innerItem) => new { Outer = grouping.Outer, Inner = innerItem });
            return ApplyExpandedSelect(leftJoinedQueryable, pair => resultSelector.Invoke(pair.Outer, pair.Inner));
        }

        [Pure]
        private static IQueryable<TResult> ApplyExpandedSelect<TInput, TResult>(IQueryable<TInput> inputs, Expression<Func<TInput, TResult>> selector)
        {
            return inputs.Select(selector.Expand());
        }
    }
}
#endif