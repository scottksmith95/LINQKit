using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
#if !NET35
using System.Threading;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
#endif

namespace LinqKit
{
    /// <summary>
    /// An IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.
    /// This is based on the excellent work of Tomas Petricek: http://tomasp.net/blog/linq-expand.aspx
    /// </summary>
#if NET35
    public class ExpandableQuery<T> : IQueryable<T>, IOrderedQueryable<T>, IOrderedQueryable
#else
    public class ExpandableQuery<T> : IQueryable<T>, IOrderedQueryable<T>, IOrderedQueryable, IDbAsyncEnumerable<T>
#endif
    {
        readonly ExpandableQueryProvider<T> _provider;
        readonly IQueryable<T> _inner;

        internal IQueryable<T> InnerQuery { get { return _inner; } }			// Original query, that we're wrapping

        internal ExpandableQuery(IQueryable<T> inner)
        {
            _inner = inner;
            _provider = new ExpandableQueryProvider<T>(this);
        }

        Expression IQueryable.Expression { get { return _inner.Expression; } }
        Type IQueryable.ElementType { get { return typeof(T); } }
        IQueryProvider IQueryable.Provider { get { return _provider; } }
        public IEnumerator<T> GetEnumerator() { return _inner.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return _inner.GetEnumerator(); }
        public override string ToString() { return _inner.ToString(); }

#if !NET35
        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            var asyncEnumerable = _inner as IDbAsyncEnumerable<T>;
            if (asyncEnumerable != null)
                return asyncEnumerable.GetAsyncEnumerator();
            return new ExpandableDbAsyncEnumerator<T>(_inner.GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return this.GetAsyncEnumerator();
        }
#endif
    }

#if !NET35
    internal class ExpandableQueryOfClass<T> : ExpandableQuery<T>
        where T: class
    {
        public ExpandableQueryOfClass(IQueryable<T> inner): base(inner)
        {
        }

        public IQueryable<T> Include(string path)
        {
            return InnerQuery.Include(path).AsExpandable();
        }
    }

    class ExpandableQueryProvider<T> : IQueryProvider, IDbAsyncQueryProvider
#else
    class ExpandableQueryProvider<T> : IQueryProvider
#endif
    {
        readonly ExpandableQuery<T> _query;

        internal ExpandableQueryProvider(ExpandableQuery<T> query)
        {
            _query = query;
        }

        // The following four methods first call ExpressionExpander to visit the expression tree, then call
        // upon the inner query to do the remaining work.

        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            return _query.InnerQuery.Provider.CreateQuery<TElement>(expression.Expand()).AsExpandable();
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            return _query.InnerQuery.Provider.CreateQuery(expression.Expand());
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            return _query.InnerQuery.Provider.Execute<TResult>(expression.Expand());
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return _query.InnerQuery.Provider.Execute(expression.Expand());
        }

#if !NET35
		public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
		{
			var asyncProvider = _query.InnerQuery.Provider as IDbAsyncQueryProvider;
			if (asyncProvider != null)
				return asyncProvider.ExecuteAsync(expression.Expand(), cancellationToken);
			return Task.FromResult(_query.InnerQuery.Provider.Execute(expression.Expand()));
		}

		public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
		{
			var asyncProvider = _query.InnerQuery.Provider as IDbAsyncQueryProvider;
			if (asyncProvider != null)
				return asyncProvider.ExecuteAsync<TResult>(expression.Expand(), cancellationToken);
			return Task.FromResult(_query.InnerQuery.Provider.Execute<TResult>(expression.Expand()));
		}
#endif
    }
}
