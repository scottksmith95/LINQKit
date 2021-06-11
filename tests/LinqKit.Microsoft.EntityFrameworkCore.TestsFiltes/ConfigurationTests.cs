using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using LinqKit.Microsoft.EntityFrameworkCore.Tests.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LinqKit.Microsoft.EntityFrameworkCore.Tests
{
    public class ConfigurationTests
    {
        [Expandable(nameof(FilterTrueImpl))]
        public static bool FilterTrue<T>(T e) => throw new NotImplementedException();

        private static Expression<Func<T, bool>> FilterTrueImpl<T>()
            => e => true;

        [Expandable(nameof(FilterFalseImpl))]
        public static bool FilterFalse<T>(T e) => throw new NotImplementedException();

        private static Expression<Func<T, bool>> FilterFalseImpl<T>()
            => e => false;


        [Fact]
        public void WithExpressionExpandingTest()
        {
            var builder = new DbContextOptionsBuilder<TestContext>();
            builder
                .UseSqlite("DataSource=testdb;mode=memory;cache=shared")
                .WithExpressionExpanding();

            using (var context = new TestContext(builder.Options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                context.TestEntities.Add(new TestEntity {Id = 1, Value = "One"});
                context.SaveChanges();

                context.TestEntities.Where(e => FilterTrue(e)).Should().HaveCount(1);
                context.TestEntities.Where(e => FilterFalse(e)).Should().HaveCount(0);
            }
        }

        [Fact]
        public void WithoutExpressionExpandingTest()
        {
            var builder = new DbContextOptionsBuilder<TestContext>();
            builder
                .UseSqlite("DataSource=testdb;mode=memory;cache=shared");

            using (var context = new TestContext(builder.Options))
            {

                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                context.TestEntities.Add(new TestEntity {Id = 1, Value = "One"});
                context.SaveChanges();

                FluentActions.Invoking(() => context.TestEntities.Where(e => FilterTrue(e)).ToArray()).Should()
                    .Throw<InvalidOperationException>();
                FluentActions.Invoking(() => context.TestEntities.Where(e => FilterFalse(e)).ToArray()).Should()
                    .Throw<InvalidOperationException>();
            }

        }

    }
}