#if EFCORE3 || EFCORE5

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;

namespace LinqKit
{
    internal class ExpandableDbContextOptionsExtension : IDbContextOptionsExtension
    {
        public DbContextOptionsExtensionInfo Info
            => new ExtensionInfo(this);

        public void ApplyServices(IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            for (var index = services.Count - 1; index >= 0; index--)
            {
                var descriptor = services[index];
                if (descriptor.ServiceType != typeof(IQueryTranslationPreprocessorFactory))
                    continue;
                if (descriptor.ImplementationType is null)
                    continue;

                // Add Injectable factory for actual implementation
                services[index] = new ServiceDescriptor(
                    descriptor.ServiceType,
                    typeof(ExpandableQueryTranslationPreprocessorFactory<>)
                        .MakeGenericType(descriptor.ImplementationType),
                    descriptor.Lifetime
                );

                // Add actual implementation as it is
                services.Add(
                    new ServiceDescriptor(
                        descriptor.ImplementationType,
                        descriptor.ImplementationType,
                        descriptor.Lifetime
                    )
                );
            }

            _ = services.AddSingleton(new ExpandableQueryTranslationPreprocessorOptions());
        }

        public void Validate(IDbContextOptions options)
        {
        }

        private class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            public override bool IsDatabaseProvider
                => false;

            public override string LogFragment
                => "LINQKitExpandable";

            public override long GetServiceProviderHashCode()
                => "LINQKitExpandable".GetHashCode();

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
            }
        }
    }
}

#endif