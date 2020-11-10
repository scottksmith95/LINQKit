using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
#if !(NET35 || WINDOWS_APP || NETSTANDARD || PORTABLE40 || UAP)
using System.Runtime.CompilerServices;
#endif

namespace LinqKit.Tests.Net452
{
    public class ExpressionCombinerTests
    {
        [Fact]
        public void ExpressionCombiner_InvokeExpressionCombiner()
        {
            Expression<Func<Tuple<int, string>, bool>> criteria1 = x => x.Item1 > 1000;
            Expression<Func<Tuple<int, string>, bool>> criteria2 = y => y.Item2.Contains("a");
            Expression<Func<Tuple<int, string>, bool>> criteria3 = criteria1.Or(z => criteria2.Invoke(z));

            Assert.Equal(
                "x => ((x.Item1 > 1000) OrElse x.Item2.Contains(\"a\"))",
                criteria3.Expand().ToString());
        }

        [Fact]
        public void ExpressionCombiner_ExpressionOrElsePredicate()
        {
            Expression<Func<bool, bool, bool, bool>> criteria = (a, b, c) => a || b || c;
            var exp = criteria.Expand().ToString();
            Assert.Equal("(a, b, c) => ((a OrElse b) OrElse c)", exp);
        }

        [Fact]
        public void ExpressionCombiner_ExpressionBalanceTest()
        {
            var exp = Expression.Equal(Expression.Constant(1), Expression.Constant(1));
            var list = Enumerable.Repeat(exp, 50000).ToArray();
            // Would throw stackoverflow with over 3100 items:
            // var combined = list.Aggregate(Expression.OrElse);
            // But this will work:
            var combined = list.AggregateBalanced(Expression.OrElse);
            var executed = combined.Expand().ToString();
            Assert.Contains("(1 == 1)", executed);
        }

        public class Foo { public Bar bar; }
        public class Bar { public bool baz; }

        [Fact]
        public void ExpressionCombiner_UnboundedVariableInExpandedPredicateTest()
        {
            Expression<Func<Foo, Bar>> barGetter = f => f.bar;
            Expression<Func<Bar, bool>> barPredicate = b => b.baz;
            Expression<Func<Foo, bool>> fooPredicate = x => barPredicate.Invoke(barGetter.Invoke(x));
            Expression<Func<Foo, bool>> inception = y => fooPredicate.Invoke(y);

            var expanded = inception.Expand(); // y => x.bar.baz
            var compiled = expanded.Compile(); // throws an InvalidOperationException
            var result = compiled.Invoke(new Foo { bar = new Bar() });
            Assert.False(result);
        }

        [Fact]
        public void ExpressionCombiner_ExpressionAsMethodVariable()
        {
            Expression<Func<Tuple<int, string>, bool>> criteria1 = x => x.Item1 > 1000;
            Expression<Func<Tuple<int, string>, bool>> criteria2 = x => criteria1.Invoke(x) || x.Item2.Contains("a");

            Assert.Equal(
                "x => ((x.Item1 > 1000) OrElse x.Item2.Contains(\"a\"))",
                criteria2.Expand().ToString());
        }

        Expression<Func<Tuple<int, string>, bool>> _ExpressionAsField_criteria1 = x => x.Item1 > 1000;

        [Fact]
        public void ExpressionCombiner_ExpressionAsField()
        {
            Expression<Func<Tuple<int, string>, bool>> criteria2 =
                x => _ExpressionAsField_criteria1.Invoke(x) || x.Item2.Contains("a");

            Assert.Equal(
                "x => ((x.Item1 > 1000) OrElse x.Item2.Contains(\"a\"))",
                criteria2.Expand().ToString());
        }

        Expression<Func<Tuple<int, string>, bool>> _ExpressionAsProperty_criteria1 => x => x.Item1 > 1000;

        [Fact]
        public void ExpressionCombiner_ExpressionAsProperty()
        {
            Expression<Func<Tuple<int, string>, bool>> criteria2 =
                x => _ExpressionAsProperty_criteria1.Invoke(x) || x.Item2.Contains("a");

            Assert.Equal(
                "x => ((x.Item1 > 1000) OrElse x.Item2.Contains(\"a\"))",
                criteria2.Expand().ToString());
        }

        private class _ExpressionAsProperty_FromExpressionVariable_criteria1
        {
            public Expression<Func<Tuple<int, string>, bool>> Expr => x => x.Item1 > 1000;
        }

        [Fact]
        public void ExpressionCombiner_ExpressionAsProperty_FromExpressionVariable()
        {
            var criteria1obj = new _ExpressionAsProperty_FromExpressionVariable_criteria1();
            Expression<Func<_ExpressionAsProperty_FromExpressionVariable_criteria1, Tuple<int, string>, bool>> getCriteria1 = (c, x) => c.Expr.Invoke(x);
            Expression<Func<Tuple<int, string>, bool>> criteria2 =
                x => getCriteria1.Invoke(criteria1obj, x) || x.Item2.Contains("a");

            Assert.Equal(
                "x => ((x.Item1 > 1000) OrElse x.Item2.Contains(\"a\"))",
                criteria2.Expand().ToString());
        }

        private class _ExpressionAsField_FromExpressionVariable_criteria1
        {
            public Expression<Func<Tuple<int, string>, bool>> Expr = x => x.Item1 > 1000;
        }

        [Fact]
        public void ExpressionCombiner_ExpressionAsField_FromExpressionVariable()
        {
            var criteria1obj = new _ExpressionAsField_FromExpressionVariable_criteria1();
            Expression<Func<_ExpressionAsField_FromExpressionVariable_criteria1, Tuple<int, string>, bool>> getCriteria1 = (c, x) => c.Expr.Invoke(x);
            Expression<Func<Tuple<int, string>, bool>> criteria2 =
                x => getCriteria1.Invoke(criteria1obj, x) || x.Item2.Contains("a");

            Assert.Equal(
                "x => ((x.Item1 > 1000) OrElse x.Item2.Contains(\"a\"))",
                criteria2.Expand().ToString());
        }

        [Fact]
        public void ExpressionCombiner_ExpressionAsParam()
        {
            Assert.Equal(
                "x => ((x.Item1 > 1000) OrElse x.Item2.Contains(\"a\"))",
                ExpressionCombiner_ExpressionAsParam_Method(x => x.Item1 > 1000));
        }

        Expression<Func<Tuple<int, string>, int>> _ExpressionAsProperty_UsedAsParam_valueExpr => x => x.Item1;

        [Fact]
        public void ExpressionCombiner_ExpressionAsProperty_UsedAsParam()
        {
            Expression<Func<Tuple<int, string>, bool>> criteria = x => _possibleValues.Contains(_ExpressionAsProperty_UsedAsParam_valueExpr.Invoke(x));

            Assert.Equal(
                "x => " + ConstExpressionString(() => _possibleValues) + ".Contains(x.Item1)",
                criteria.Expand().ToString());
        }

        private string ExpressionCombiner_ExpressionAsParam_Method(Expression<Func<Tuple<int, string>, bool>> criteria1)
        {
            Expression<Func<Tuple<int, string>, bool>> criteria2 =
                x => criteria1.Invoke(x) || x.Item2.Contains("a");

            return criteria2.Expand().ToString();
        }

        private int[] _possibleValues = new int[] { 1, 2, 3 };

        [Fact]
        public void ExpressionCombiner_ExpressionAsVariable_UsedAsParam()
        {
            Expression<Func<Tuple<int, string>, int>> valueExpr = x => x.Item1;
            Expression<Func<Tuple<int, string>, bool>> criteria = x => _possibleValues.Contains(valueExpr.Invoke(x));

            Assert.Equal(
                "x => " + ConstExpressionString(() => _possibleValues) + ".Contains(x.Item1)",
                criteria.Expand().ToString());
        }

        Expression<Func<Tuple<int, string>, int>> _ExpressionAsVariable_UsedAsParam_valueExpr = x => x.Item1;

        [Fact]
        public void ExpressionCombiner_ExpressionAsField_UsedAsParam()
        {
            Expression<Func<Tuple<int, string>, bool>> criteria = x => _possibleValues.Contains(_ExpressionAsVariable_UsedAsParam_valueExpr.Invoke(x));

            Assert.Equal(
                "x => " + ConstExpressionString(() => _possibleValues) + ".Contains(x.Item1)",
                criteria.Expand().ToString());
        }

        [Fact]
        public void ExpressionCombiner_ExpressionAsParam_UsedAsParam()
        {
            Assert.Equal(
                "x => " + ConstExpressionString(() => _possibleValues) + ".Contains(x.Item1)",
                ExpressionAsParam_UsedAsParam_Method(x => x.Item1));
        }

        private string ExpressionAsParam_UsedAsParam_Method(Expression<Func<Tuple<int, string>, int>> valueExpr)
        {
            Expression<Func<Tuple<int, string>, bool>> criteria = x => _possibleValues.Contains(valueExpr.Invoke(x));

            return criteria.Expand().ToString();
        }

        [Fact]
        public void NestingMemberAccess()
        {
            Expression<Func<Tuple<Tuple<int, DateTime>, string>, Tuple<int, DateTime>>> memberExpr1 = x => x.Item1;
            Expression<Func<Tuple<int, DateTime>, DateTime>> memberExpr2 = x => x.Item2;

            Expression<Func<Tuple<Tuple<int, DateTime>, string>, DateTime>> criteria =
                x => memberExpr2.Invoke(memberExpr1.Invoke(x));

            Assert.Equal(
                "x => x.Item1.Item2",
                criteria.Expand().ToString());
        }

        [Fact]
        public void ExpandProcessesArguments()
        {
            Expression<Func<Tuple<bool, bool>, bool>> expr1 = x => x.Item1 && x.Item2;
            Expression<Func<bool, Tuple<bool, bool>>> expr2 = y => new Tuple<bool, bool>(y, y);
            Expression<Func<bool, bool>> nestedExpression = z => expr1.Invoke(expr2.Invoke(z));

            Assert.Equal(
                nestedExpression.Expand().ToString(),
                nestedExpression.Expand().Expand().ToString());
        }

        private string ConstExpressionString<TResult>(Expression<Func<TResult>> expr)
        {
            return expr.ToString().Substring(6);
        }
    }
}