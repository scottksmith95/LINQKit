using System;
using System.Linq.Expressions;
using Xunit;

namespace LinqKit.Tests.Net452
{
    public class ExpressionStarterTests
    {
        private class User
        {
            public int Id1 { get; set; }
            public int Id2 { get; set; }
        }

        [Fact]
        public void ExpressionStarter_Null()
        {
            ExpressionStarter<string> predicate = null;
            Assert.Null(predicate.Expand());
        }

        [Fact]
        public void ExpressionStarter_Normal()
        {
            var predicate = PredicateBuilder.New<string>(s => s == "a");
            Assert.Equal("s => (s == \"a\")", predicate.Expand().ToString());
        }

        [Fact]
        public void ExpressionStarter_CanBeAssignedFromExpression()
        {
            ExpressionStarter<string> predicate = (Expression<Func<string, bool>>)(s => s == "a");
            Assert.Equal("s => (s == \"a\")", predicate.Expand().ToString());
        }

        [Fact]
        public void ExpressionStarter_ExtendWithExpression()
        {
            var first = PredicateBuilder.New<User>(s => s.Id1 == 1);
            Expression<Func<User, bool>> second = y => y.Id2 == 2;

            var predicate = first.Extend(second);
            Assert.Equal("s => ((s.Id1 == 1) OrElse (s.Id2 == 2))", predicate.Expand().ToString());
        }

        [Fact]
        public void ExpressionStarter_ExtendWithExpression_And()
        {
            var first = PredicateBuilder.New<User>(s => s.Id1 == 1);
            Expression<Func<User, bool>> second = y => y.Id2 == 2;

            var predicate = first.Extend(second, PredicateOperator.And);
            Assert.Equal("s => ((s.Id1 == 1) AndAlso (s.Id2 == 2))", predicate.Expand().ToString());
        }

        [Fact]
        public void ExpressionStarter_ExtendWithExpressionStarter()
        {
            var first = PredicateBuilder.New<User>(s => s.Id1 == 1);
            var second = PredicateBuilder.New<User>(y => y.Id2 == 2);

            var predicate = first.Extend(second);
            Assert.Equal("s => ((s.Id1 == 1) OrElse (s.Id2 == 2))", predicate.Expand().ToString());
        }
    }
}