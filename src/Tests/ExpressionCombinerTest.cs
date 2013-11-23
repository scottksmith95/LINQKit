using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using LinqKit;

namespace LinqKit.Tests
{
    public class ExpressionCombinerTest
    {
        [Fact]
        public void Expand()
        {
            Expression<Func<Tuple<int, string>, bool>> criteria1 = x => x.Item1 > 1000;
            Expression<Func<Tuple<int, string>, bool>> criteria2 = x => criteria1.Invoke(x) || x.Item2.Contains("a");

            Assert.Equal(
                "x => ((x.Item1 > 1000) OrElse x.Item2.Contains(\"a\"))",
                criteria2.Expand().ToString());
        }

        private int[] _possibleValues = new int[] { 1, 2, 3 };

        [Fact]
        public void Expand_ExpressionAsParam()
        {
            Expression<Func<Tuple<int, string>, int>> valueExpr = x => x.Item1;
            Expression<Func<Tuple<int, string>, bool>> criteria = x => _possibleValues.Contains(valueExpr.Invoke(x));

            Assert.Equal(
                "x => " + ConstExpressionString(() => _possibleValues) + ".Contains(x.Item1)",
                criteria.Expand().ToString());
        }

        Expression<Func<Tuple<int, string>, int>> _Expand_ExpressionAsParamAndField_valueExpr = x => x.Item1;

        [Fact]
        public void Expand_ExpressionAsParamAndField()
        {
            Expression<Func<Tuple<int, string>, bool>> criteria = x => _possibleValues.Contains(_Expand_ExpressionAsParamAndField_valueExpr.Invoke(x));

            Assert.Equal(
                "x => " + ConstExpressionString(() => _possibleValues) + ".Contains(x.Item1)",
                criteria.Expand().ToString());
        }

        private string ConstExpressionString<TResult>(Expression<Func<TResult>> expr)
        {
            return expr.ToString().Substring(6);
        }
    }
}
