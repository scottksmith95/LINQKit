﻿#if !(NET35 || NET40 || NOEF)
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#if EF
using System.Data.Entity.Infrastructure;
#endif

namespace LinqKit
{
    /// <summary> Class for async-await style list enumeration support (e.g. .ToListAsync())</summary>
    public sealed class ExpandableDbAsyncEnumerator<T> : IDisposable,
#if EFCORE
        IAsyncEnumerator<T>
#else
        IDbAsyncEnumerator<T>
#endif
    {
        private readonly IEnumerator<T> _inner;

        /// <summary> Class for async-await style list enumeration support (e.g. .ToListAsync())</summary>
        public ExpandableDbAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        /// <summary> Dispose, .NET using-pattern </summary>
        public void Dispose()
        {
            _inner.Dispose();
        }

        /// <summary> Enumerator-pattern: MoveNext </summary>
        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }

#if EFCORE
        /// <summary> Enumerator-pattern: Current item </summary>
        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }
#endif
        /// <summary> Enumerator-pattern: Current item </summary>
        public T Current
        {
            get { return _inner.Current; }
        }

#if !EFCORE
        object IDbAsyncEnumerator.Current
        {
            get { return Current; }
        }
#endif
    }
}
#endif