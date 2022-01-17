using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace LinqKit.Tests.Net452
{
    public class ExpressionExpanderTests
    {
        [Fact]
        public void ExpressionCombiner_Expression_Subtract()
        {
            var exp = Expression.Subtract(Expression.Constant(5), Expression.Constant(2));
            var executed = exp.Expand().ToString();
            Assert.Equal(exp.ToString(), executed);
        }

        [Fact]
        public void ExpressionCombiner_Expression_Power()
        {
            var exp = Expression.Power(Expression.Constant(5.1), Expression.Constant(2.5));
            var executed = exp.Expand().ToString();
            Assert.Equal(exp.ToString(), executed);
        }

        [Fact]
        public void ExpressionCombiner_Expression_UnaryPlus()
        {
            var exp = Expression.UnaryPlus(Expression.Constant(5));
            var executed = exp.Expand().ToString();
            Assert.Equal(exp.ToString(), executed);
        }

        [Fact]
        public void ExpressionExpander_Expression_Index()
        {
            var listParameter = Expression.Parameter(typeof(List<string>), "l");
            Expression<Func<List<string>, string>> expression1 = Expression.Lambda<Func<List<string>, string>>(Expression.MakeIndex(
                    listParameter,
                    typeof(List<string>).GetProperties().SingleOrDefault(p => p.GetIndexParameters().Length > 0),
                    new[] { Expression.Constant(0) }),
                listParameter);
            Expression<Func<string, string>> expression2 = s => s;

            var executed = Linq.Expr((List<string> l) => expression2.Invoke(expression1.Invoke(l))).Expand().ToString();
            Assert.Equal(expression1.ToString(), executed);
        }
    }
}
