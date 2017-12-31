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
    }
}
