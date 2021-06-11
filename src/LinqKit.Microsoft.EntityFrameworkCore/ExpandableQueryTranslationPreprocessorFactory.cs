#if EFCORE3 || EFCORE5

using System;
using Microsoft.EntityFrameworkCore.Query;

namespace LinqKit
{
    internal class ExpandableQueryTranslationPreprocessorFactory<TInnerFactory> : IQueryTranslationPreprocessorFactory
        where TInnerFactory : class, IQueryTranslationPreprocessorFactory
    {
        private readonly ExpandableQueryTranslationPreprocessorOptions _options;
        private readonly QueryTranslationPreprocessorDependencies _dependencies;
        private readonly TInnerFactory _innerFactory;

        public ExpandableQueryTranslationPreprocessorFactory(ExpandableQueryTranslationPreprocessorOptions options,
            QueryTranslationPreprocessorDependencies dependencies,
            TInnerFactory innerFactory)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            _innerFactory = innerFactory ?? throw new ArgumentNullException(nameof(innerFactory));
        }

        public QueryTranslationPreprocessor Create(QueryCompilationContext queryCompilationContext)
            => new ExpandableQueryTranslationPreprocessor(_options, _dependencies, queryCompilationContext, _innerFactory.Create(queryCompilationContext));
    }
}

#endif