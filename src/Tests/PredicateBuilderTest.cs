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
    } 
}
