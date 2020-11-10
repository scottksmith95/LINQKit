using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace LinqKit.Tests.Net452
{
    public class DeepInvocationTests
    {
        public class Repository
        {
            public class SampleDocument
            {
                public SampleDocument(Guid id)
                {
                    Id = id;
                }

                public Guid Id { get; }
            }

            public class AuditDocument<T>
            {
                public AuditDocument(T entity, bool active)
                {
                    Entity = entity;
                    Active = active;
                }

                public T Entity { get; }
                public bool Active { get; }
            }

            private readonly IQueryable<AuditDocument<SampleDocument>> _inMemoryStore = new List<AuditDocument<SampleDocument>>
            {
                new AuditDocument<SampleDocument>(new SampleDocument(new Guid("08d87544-4aae-8e73-afd5-772ab0a086a1")), true),
                new AuditDocument<SampleDocument>(new SampleDocument(new Guid("08d8754a-fb8f-8fe8-afd5-772770f5e423")), false)
            }
            .AsQueryable();

            public SampleDocument GetDocument(Expression<Func<SampleDocument, bool>> expression)
            {
                return _inMemoryStore.FirstOrDefault(EntityFilter(expression))?.Entity;
            }

            protected static Expression<Func<AuditDocument<SampleDocument>, bool>> EntityFilter(Expression<Func<SampleDocument, bool>> filter)
            {
                return PredicateBuilder
                    .New(PropFilter<AuditDocument<SampleDocument>, SampleDocument>(ae => ae.Entity, filter))
                    .And(ad => ad.Active)
                    .Expand();
            }

            private static Expression<Func<TParent, bool>> PropFilter<TParent, TChild>(Expression<Func<TParent, TChild>> selector, Expression<Func<TChild, bool>> filter)
            {
                return Expression.Lambda<Func<TParent, bool>>(Expression.Invoke(filter, selector.Body),
                    selector.Parameters);
            }
        }

        [Fact]
        public void TestPredicateBuilder()
        {
            var repository = new Repository();

            var activeDoc = repository.GetDocument(sd => sd.Id == new Guid("08d87544-4aae-8e73-afd5-772ab0a086a1"));
            Assert.NotNull(activeDoc);

            var inactiveDoc = repository.GetDocument(sd => sd.Id == new Guid("08d8754a-fb8f-8fe8-afd5-772770f5e423"));
            Assert.Null(inactiveDoc);
        }
    }
}