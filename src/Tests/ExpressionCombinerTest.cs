using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Xunit;
using LinqKit;

namespace LinqKit.Tests
{
    public class ExpressionCombinerTest
    {
        [Fact]
        public void ExpressionAsMethodVariable()
        {
            Expression<Func<Tuple<int, string>, bool>> criteria1 = x => x.Item1 > 1000;
            Expression<Func<Tuple<int, string>, bool>> criteria2 = x => criteria1.Invoke(x) || x.Item2.Contains("a");

            Assert.Equal(
                "x => ((x.Item1 > 1000) OrElse x.Item2.Contains(\"a\"))",
                criteria2.Expand().ToString());
        }

        Expression<Func<Tuple<int, string>, bool>> _ExpressionAsField_criteria1 = x => x.Item1 > 1000;

        [Fact]
        public void ExpressionAsField()
        {
            Expression<Func<Tuple<int, string>, bool>> criteria2 = 
                x => _ExpressionAsField_criteria1.Invoke(x) || x.Item2.Contains("a");

            Assert.Equal(
                "x => ((x.Item1 > 1000) OrElse x.Item2.Contains(\"a\"))",
                criteria2.Expand().ToString());
        }

        [Fact]
        public void ExpressionAsParam()
        {
            Assert.Equal(
                "x => ((x.Item1 > 1000) OrElse x.Item2.Contains(\"a\"))",
                ExpressionAsParam_Method(x => x.Item1 > 1000));
        }

        private string ExpressionAsParam_Method(Expression<Func<Tuple<int, string>, bool>> criteria1)
        {
            Expression<Func<Tuple<int, string>, bool>> criteria2 =
                x => criteria1.Invoke(x) || x.Item2.Contains("a");

            return criteria2.Expand().ToString();
        }

        private int[] _possibleValues = new int[] { 1, 2, 3 };

        [Fact]
        public void ExpressionAsVariable_UsedAsParam()
        {
            Expression<Func<Tuple<int, string>, int>> valueExpr = x => x.Item1;
            Expression<Func<Tuple<int, string>, bool>> criteria = x => _possibleValues.Contains(valueExpr.Invoke(x));

            Assert.Equal(
                "x => " + ConstExpressionString(() => _possibleValues) + ".Contains(x.Item1)",
                criteria.Expand().ToString());
        }

        Expression<Func<Tuple<int, string>, int>> _ExpressionAsVariable_UsedAsParam_valueExpr = x => x.Item1;

        [Fact]
        public void ExpressionAsField_UsedAsParam()
        {
            Expression<Func<Tuple<int, string>, bool>> criteria = x => _possibleValues.Contains(_ExpressionAsVariable_UsedAsParam_valueExpr.Invoke(x));

            Assert.Equal(
                "x => " + ConstExpressionString(() => _possibleValues) + ".Contains(x.Item1)",
                criteria.Expand().ToString());
        }

        [Fact]
        public void ExpressionAsParam_UsedAsParam()
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
