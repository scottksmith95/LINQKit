#if EFCORE3 || EFCORE5

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LinqKit
{
    /// <summary>
    /// Replaces method calls with lambda expressions.
    /// </summary>
    public static class ExpandableDbContextOptionsBuilderExtensions
    {
        /// <summary>
        /// Replaces method calls with lambda expressions.
        /// </summary>
        /// <param name="optionsBuilder">The builder being used to configure the context.</param>
        /// <returns>The options builder so that further configuration can be chained.</returns>
        public static DbContextOptionsBuilder WithExpressionExpanding(this DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder is null)
                throw new ArgumentNullException(nameof(optionsBuilder));

            var extension = GetOrCreateExtension(optionsBuilder);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return optionsBuilder;
        }

        private static ExpandableDbContextOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.Options.FindExtension<ExpandableDbContextOptionsExtension>()
               ?? new ExpandableDbContextOptionsExtension();
    }
}

#endif