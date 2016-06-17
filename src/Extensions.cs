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
#if !(NET35 || NET40)
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
        public static IQueryable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer, IEnumerable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            //+ Types
            var typeOuter = typeof(TOuter);
            var typeInner = typeof(TInner);
            var typeKey = typeof(TKey);
            var typeResult = typeof(TResult);
            var typeIEnumerableInner = typeof(IEnumerable<TInner>);
            //Dynamic Type: T<TOuter, IEnumerable<TInner>>
            var typeGroupJoinResult = new { Outer = default(TOuter), IEnumerableInner = default(IEnumerable<TInner>) }.GetType();
            //Dynamic Type: T<TOuter, TInner>
            var typeSelectManyResult = new { Outer = default(TOuter), Inner = default(TInner) }.GetType();

            //+ Methods
            var methodsQueryable = typeof(Queryable).GetMethods();
            var methodsEnumerable = typeof(Enumerable).GetMethods();
            //Dynamic Type Constructor: T<TOuter, IEnumerable<TInner>>(TOuter outer, IEnumerable<TInner> IEnumerableInner)
            var methodGroupJoinResultTypeConstructor = typeGroupJoinResult.GetConstructors().First();
            //Dynamic Type Constructor: T<TOuter, TInner>(TOuter outer, TInner Inner)
            var methodSelectManyResultTypeConstructor = typeSelectManyResult.GetConstructors().First();
            //.GroupJoin<TOuter, TInner, TKey, T<TOuter, IEnumerable<TInner>>>(...)
            var methodGroupJoin = methodsQueryable.First(m => m.Name == nameof(Queryable.GroupJoin) && m.GetParameters().Length == 5)
                .MakeGenericMethod(typeOuter, typeInner, typeKey, typeGroupJoinResult);
            //.DefaultIfEmpty<TInner>()
            var methodDefaultIfEmpty = methodsEnumerable.First(m => m.Name == nameof(Enumerable.DefaultIfEmpty) && m.GetParameters().Length == 1)
                .MakeGenericMethod(typeInner);
            //.SelectMany<T<TOuter, IEnumerable<TInner>>, TInner, T<TOuter, TInner>>()
            var methodSelectMany = methodsQueryable.Where(m => m.Name == nameof(Queryable.SelectMany) && m.GetParameters().Length == 2)
                .OrderBy(m => m.ToString().Length).First().MakeGenericMethod(typeGroupJoinResult, typeSelectManyResult);
            //.Select<TInner, T<TOuter, TInner>>()
            var methodSelectIEnumerableInner = methodsEnumerable.Where(m => m.Name == nameof(Enumerable.Select) && m.GetParameters().Length == 2)
                .OrderBy(m => m.ToString().Length).First().MakeGenericMethod(typeInner, typeSelectManyResult);
            //.Select<T<TOuter, TInner>, TResult>()
            var methodSelectResult = methodsQueryable.Where(m => m.Name == nameof(Queryable.Select) && m.GetParameters().Length == 2)
                .OrderBy(m => m.ToString().Length).First().MakeGenericMethod(typeSelectManyResult, typeResult);
            //.Invoke<TOuter, TInner, TResult>()
            var methodInvoke = typeof(Extensions).GetMethods().First(m => m.Name == nameof(Extensions.Invoke) && m.GetParameters().Length == 3)
                .MakeGenericMethod(typeOuter, typeInner, typeResult);

            //+ GroupJoin Parameters
            var parameterOuter = Expression.Parameter(typeOuter);
            var parameterIEnumerableInner = Expression.Parameter(typeIEnumerableInner);

            //+ GroupJoin Expressions
            var expressionGroupJoinResultArguments = new Expression[]
            {
                parameterOuter,
                //IEnumerableInner.DefaultIfEmpty()
                Expression.Call(null, methodDefaultIfEmpty, parameterIEnumerableInner)
            };

            //new { Outer = Outer, IEnumerableInner = IEnumerableInner.DefaultIfEmpty()}
            var expressionGroupJoinResult = Expression.New(methodGroupJoinResultTypeConstructor, expressionGroupJoinResultArguments, typeGroupJoinResult.GetProperties());
            //(Outer, IEnumerableInner) = > new { Outer = Outer, IEnumerableInner = IEnumerableInner.DefaultIfEmpty()}
            var groupJoinResult = Expression.Lambda(expressionGroupJoinResult, new[] { parameterOuter, parameterIEnumerableInner });

            //+ GroupJoin Invoke
            //outer.GroupJoin<TOuter, TInner, TKey, T<TOuter, IEnumerable<TInner>>>(inner, outerKeySelector, innerKeySelector, (o, i) => new { Outer = o, IEnumerableInner = i.DefaultIfEmpty() })
            var groupJoin = methodGroupJoin.Invoke(null, new Object[] { outer, inner, outerKeySelector, innerKeySelector, groupJoinResult });

            //+ SelectMany Parameters
            var parameterGroupJoinResult = Expression.Parameter(typeGroupJoinResult);
            var parameterInner = Expression.Parameter(typeInner);

            //+ SelectMany Expressions
            var expressionSelectResultArguments = new Expression[]
            {
                //groupJoinResult.Outer
                Expression.MakeMemberAccess(parameterGroupJoinResult, typeGroupJoinResult.GetProperty("Outer")),
                parameterInner
            };
            var expressionSelectManyResultArguments = new Expression[]
            {
                //groupJoinResult.IEnumerableInner
                Expression.MakeMemberAccess(parameterGroupJoinResult, typeGroupJoinResult.GetProperty("IEnumerableInner")),
                //i => new { Outer = groupJoinResult.Outer, Inner = i }
                Expression.Lambda(Expression.New(methodSelectManyResultTypeConstructor, expressionSelectResultArguments, typeSelectManyResult.GetProperties()), new [] { parameterInner })
            };
            //groupJoinResult => groupJoinResult.IEnumerableInner.Select(i => new { Outer = groupJoinResult.Outer, Inner = i })
            var selectManyResult = Expression.Lambda(Expression.Call(null, methodSelectIEnumerableInner, expressionSelectManyResultArguments), new[] { parameterGroupJoinResult });

            //+ SelectMany Invoke
            //groupJoin.SelectMany<T<TOuter, IEnumerable<TInner>>, T<TOuter, TInner>>()
            var selectMany = methodSelectMany.Invoke(null, new object[] { groupJoin, selectManyResult });

            //+ Select Parameters
            var parameterSelectManyResult = Expression.Parameter(typeSelectManyResult);
            var propOuter = Expression.MakeMemberAccess(parameterSelectManyResult, typeSelectManyResult.GetProperty("Outer"));
            var propInner = Expression.MakeMemberAccess(parameterSelectManyResult, typeSelectManyResult.GetProperty("Inner"));

            //+ Select Expressions
            //resultSelector.Invoke(selectManyResult.Outer, selectManyResult.Inner)
            var invokeResult = Expression.Lambda(Expression.Call(null, methodInvoke, new Expression[] { resultSelector, propOuter, propInner }), parameterSelectManyResult);

            //+ Select Invoke
            //selectMany.Select(selectManyResult => resultSelector.Invoke(selectManyResult.Outer, selectManyResult.Inner).Expand())
            return methodSelectResult.Invoke(null, new object[] { selectMany, invokeResult.Expand() }) as IQueryable<TResult>;
        }

        /// <summary> Default side-effect style enumeration </summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var element in source)
                action(element);
        }

#endif
    }
}