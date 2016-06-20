using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using LinqKit;

namespace LinqKit.Tests
{
    public class PredicateBuilderTest
    {
        [Fact]
        public void InvokeExpressionCombiner()
        {
            Expression<Func<Tuple<int, string>, bool>> criteria1 = x => x.Item1 > 1000;
            Expression<Func<Tuple<int, string>, bool>> criteria2 = y => y.Item2.Contains("a");
            Expression<Func<Tuple<int, string>, bool>> criteria3 = criteria1.Or(z => criteria2.Invoke(z));

            Assert.Equal(
                "x => ((x.Item1 > 1000) OrElse x.Item2.Contains(\"a\"))",
                criteria3.Expand().ToString());
        }

        [Fact]
        public void ExpressionOrElsePredicate()
        {
            Expression<Func<bool, bool, bool, bool>> criteria = (a, b, c) => a || b || c;
            var exp = criteria.Expand().ToString();
            Assert.Equal("(a, b, c) => ((a OrElse b) OrElse c)", exp);
        }

        [Fact]
        public void ExpressionBalanceTest()
        {
            var exp = Expression.Equal(Expression.Constant(1), Expression.Constant(1));
            var list = Enumerable.Repeat(exp, 50000).ToArray();
            // Would throw stackoverflow with over 3100 items:
            //var combined = list.Aggregate(Expression.OrElse);
            // But this will work:
            var combined = list.AggregateBalanced(Expression.OrElse);
            var executed = combined.Expand().ToString();
            Assert.Contains("(1 == 1)", executed);
        }

        public class Foo { public Bar bar; }
        public class Bar { public bool baz; }

        [Fact]
        public void UnboundedVariableInExpandedPredicateTest()
        {
            Expression<Func<Foo, Bar>> barGetter = f => f.bar;
            Expression<Func<Bar, bool>> barPredicate = b => b.baz;
            Expression<Func<Foo, bool>> fooPredicate = x => barPredicate.Invoke(barGetter.Invoke(x));
            Expression<Func<Foo, bool>> inception = y => fooPredicate.Invoke(y);

            var expanded = inception.Expand(); // y => x.bar.baz
            var compiled = expanded.Compile(); // throws an InvalidOperationException
            var result = compiled.Invoke(new Foo{bar = new Bar()});
            Assert.False(result);
        }
    } 
}
