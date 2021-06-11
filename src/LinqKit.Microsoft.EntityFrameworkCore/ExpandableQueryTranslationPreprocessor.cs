#if EFCORE3 || EFCORE5

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace LinqKit
{
    internal class ExpandableQueryTranslationPreprocessor : QueryTranslationPreprocessor
    {
        private readonly ExpandableQueryTranslationPreprocessorOptions _options;
        private readonly QueryTranslationPreprocessor _innerPreprocessor;

        public ExpandableQueryTranslationPreprocessor(ExpandableQueryTranslationPreprocessorOptions options,
            QueryTranslationPreprocessorDependencies dependencies,
            QueryCompilationContext queryCompilationContext,
            QueryTranslationPreprocessor innerPreprocessor)
            : base(dependencies, queryCompilationContext)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _innerPreprocessor = innerPreprocessor ?? throw new ArgumentNullException(nameof(innerPreprocessor));
        }

        public override Expression Process(Expression query)
        {
            var expander = new ExpressionExpander();

            return _innerPreprocessor.Process(expander.Visit(query));
        }
    }
}

#endif