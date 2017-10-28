using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace LinqKit.Tests.Net452
{
    public class PredicateBuilderTests
    {
        private class User
        {
            public int Id1 { get; set; }
            public int Id2 { get; set; }
        }

        [Fact]
        public void PredicateBuilder_Extend()
        {
            Expression<Func<User, bool>> first = x => x.Id1 > 1;
            Expression<Func<User, bool>> second = y => y.Id2 > 2;

            var predicate = first.Extend(second);
            Assert.Equal("x => ((x.Id1 > 1) OrElse (x.Id2 > 2))", predicate.Expand().ToString());
        }

        [Fact]
        public void PredicateBuilder_Extend_And()
        {
            Expression<Func<User, bool>> first = x => x.Id1 > 1;
            Expression<Func<User, bool>> second = y => y.Id2 > 2;

            var predicate = first.Extend(second, PredicateOperator.And);
            Assert.Equal("x => ((x.Id1 > 1) AndAlso (x.Id2 > 2))", predicate.Expand().ToString());
        }

        [Fact]
        public void PredicateBuilder_PredicateNull()
        {
            var predicate = PredicateBuilder.New<string>();
            Assert.Equal("f => False", predicate.Expand().ToString());
        }

        [Fact]
        public void PredicateBuilder_PredicateNullUseDefaultExpression()
        {
            var predicate = PredicateBuilder.New<string>(true);
            Assert.Equal("f => True", predicate.Expand().ToString());
        }

        [Fact]
        public void PredicateBuilder_CanBeAssignedToExpression()
        {
            var predicate = PredicateBuilder.New<string>();
            predicate.Start(s => s == "a");

            Expression<Func<string, bool>> exp = predicate;
            Assert.Equal("s => (s == \"a\")", predicate.Expand().ToString());
            Assert.Equal("s => (s == \"a\")", exp.Expand().ToString());
        }

        [Fact]
        public void PredicateBuilder_PredicateNullOr()
        {
            var predicate = PredicateBuilder.New<string>();
            predicate.Or(s => s == "a");
            Assert.Equal("s => (s == \"a\")", predicate.Expand().ToString());
        }

        [Fact]
        public void PredicateBuilder_PredicateNullAnd()
        {
            var predicate = PredicateBuilder.New<string>();
            predicate.And(s => s == "a");
            Assert.Equal("s => (s == \"a\")", predicate.Expand().ToString());
        }

        [Fact]
        public void PredicateBuilder_PredicateOr()
        {
            // Arrange
            Expression<Func<string, bool>> expectedPredicate = s => s == "a";
            expectedPredicate = expectedPredicate.Or(s => s == "b");
            var predicate = PredicateBuilder.New<string>(s => s == "a");

            // Act
            predicate.Or(s => s == "b");

            // Assert
            var expected = expectedPredicate.Expand().ToString();
            var actual = predicate.Expand().ToString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PredicateBuilder_PredicateOrAssignment()
        {
            // Arrange
            Expression<Func<string, bool>> expectedPredicate = s => s == "a";
            expectedPredicate = expectedPredicate.Or(s => s == "b");
            var predicate = PredicateBuilder.New<string>(s => s == "a");

            // Act
            predicate = predicate.Or(s => s == "b");

            // Assert
            var expected = expectedPredicate.Expand().ToString();
            var actual = predicate.Expand().ToString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PredicateBuilder_PredicateOrUsage()
        {
            // Arrange
            var list = new List<string> { "a", "b", "c" };
            var predicate = PredicateBuilder.New<string>(s => s == "a");

            // Act
            predicate = predicate.Or(s => s == "b");

            // Assert
            var items = list.Where(predicate).ToList();
            Assert.Equal(2, items.Count);
            Assert.Equal("a", items[0]);
            Assert.Equal("b", items[1]);
        }

        [Fact]
        public void PredicateBuilder_PredicateAnd()
        {
            // Arrange
            Expression<Func<string, bool>> expectedPredicate = s => s == "a";
            expectedPredicate = expectedPredicate.And(s => s == "b");
            var predicate = PredicateBuilder.New<string>(s => s == "a");

            // Act
            predicate.And(s => s == "b");

            // Assert
            var expected = expectedPredicate.Expand().ToString();
            var actual = predicate.Expand().ToString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PredicateBuilder_PredicateAndAssignment()
        {
            // Arrange
            Expression<Func<string, bool>> expectedPredicate = s => s == "a";
            expectedPredicate = expectedPredicate.And(s => s == "b");
            var predicate = PredicateBuilder.New<string>(s => s == "a");

            // Act
            predicate = predicate.And(s => s == "b");

            // Assert
            var expected = expectedPredicate.Expand().ToString();
            var actual = predicate.Expand().ToString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PredicateBuilder_PredicateAndUsage()
        {
            // Arrange
            var list = new List<string> { "a", "b", "c" };
            var predicate = PredicateBuilder.New<string>(s => s == "a");

            // Act
            predicate = predicate.And(s => s == "b");

            // Assert
            var items = list.Where(predicate).ToList();
            Assert.Empty(items);
        }
    }
}