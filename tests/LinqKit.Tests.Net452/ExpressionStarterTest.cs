using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace LinqKit.Tests.Net452
{
    public class ExpressionStarterTest
    {
        [Fact]
        public void ExpressionStarterNull()
        {
            ExpressionStarter<string> predicate = null;
            Assert.Null(predicate.Expand());
        }

        [Fact]
        public void PredicateNull()
        {
            var predicate = PredicateBuilder.New<string>();
            Assert.Null(predicate.Expand());
        }

        [Fact]
        public void PredicateNullUseDefaultExpression()
        {
            var predicate = PredicateBuilder.New<string>(true);
            Assert.Equal("f => True", predicate.Expand().ToString());
        }

        [Fact]
        public void CanBeAssignedToExpression()
        {
            var predicate = PredicateBuilder.New<string>();
            predicate.Start(s => s == "a");
            Expression<Func<string, bool>> exp = predicate;
            Assert.Equal("s => (s == \"a\")", predicate.Expand().ToString());
            Assert.Equal("s => (s == \"a\")", exp.Expand().ToString());
        }

        [Fact]
        public void CanBeAssignedFromExpression()
        {
            ExpressionStarter<string> predicate = (Expression<Func<string, bool>>)((s) => s == "a");
            Assert.Equal("s => (s == \"a\")", predicate.Expand().ToString());
        }

        [Fact]
        public void PredicateNullOr()
        {
            var predicate = PredicateBuilder.New<string>();
            predicate.Or(s => s == "a");
            Assert.Equal("s => (s == \"a\")", predicate.Expand().ToString());
        }

        [Fact]
        public void PredicateNullAnd()
        {
            var predicate = PredicateBuilder.New<string>();
            predicate.And(s => s == "a");
            Assert.Equal("s => (s == \"a\")", predicate.Expand().ToString());
        }

        [Fact]
        public void PredicateOr()
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
        public void PredicateOrAssignment()
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
        public void PredicateOrUsage()
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
        public void PredicateAnd()
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
        public void PredicateAndAssignment()
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
        public void PredicateAndUsage()
        {
            // Arrange
            var list = new List<string> { "a", "b", "c" };
            var predicate = PredicateBuilder.New<string>(s => s == "a");

            // Act
            predicate = predicate.And(s => s == "b");

            // Assert
            var items = list.Where(predicate).ToList();
            Assert.Equal(0, items.Count);
        }
    }
}
