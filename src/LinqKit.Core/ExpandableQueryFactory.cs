//using System;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;

//namespace LinqKit
//{
//#if !NOEF && !(NET35 || NOEF || NOASYNCPROVIDER)
//        internal static class ExpandableQueryFactory<T>
//        {
//            public static readonly Func<IQueryable<T>, Func<Expression, Expression>, ExpandableQuery<T>> Create;

//            static ExpandableQueryFactory()
//            {
//                if (!typeof(T).GetTypeInfo().IsClass)
//                {
//                    Create = (query, optimizer) => new ExpandableQuery<T>(query, optimizer);
//                    return;
//                }

//                Type queryType = typeof(IQueryable<T>);
//                Type optimizerType = typeof(Func<Expression, Expression>);

//                var ctorInfo = typeof(ExpandableQueryOfClass<>).MakeGenericType(typeof(T)).GetConstructor(new[] { queryType, optimizerType });

//                var queryParam = Expression.Parameter(queryType);
//                var optimizerParam = Expression.Parameter(optimizerType);

//                var newExpr = Expression.New(ctorInfo, queryParam, optimizerParam);
//                var createExpr = Expression.Lambda<Func<IQueryable<T>, Func<Expression, Expression>, ExpandableQuery<T>>>(newExpr, queryParam, optimizerParam);

//                Create = createExpr.Compile();
//            }
//        }
//#endif
//}