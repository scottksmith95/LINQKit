using System;

namespace LinqKit
{
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary> Class for async-await style list enumeration support (e.g. .ToListAsync())</summary>
    public sealed class ExpandableDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>, IDisposable
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

        /// <summary> Enumerator-pattern: Current item </summary>
        public T Current
        {
            get { return _inner.Current; }
        }
        object IDbAsyncEnumerator.Current
        {
            get { return Current; }
        }
    }
}
