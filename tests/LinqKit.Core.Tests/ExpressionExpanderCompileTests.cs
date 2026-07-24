using System;
using System.Linq.Expressions;
using FluentAssertions;
using LinqKit;
using Xunit;

namespace LinqKitCoreTests;

public class ExpressionExpanderCompileTests
{
    private class FilterHolder
    {
        // Deliberately named like LambdaExpression.Compile, but declared on a non-generic type.
        public Func<int, bool> Compile() => x => x > 2;
    }

    [Fact]
    public void Expand_InvocationOfCompileMethodOnNonGenericType_DoesNotThrow()
    {
        var holder = new FilterHolder();
        Expression<Func<int, bool>> expr = x => holder.Compile()(x);

        var expanded = expr.Expand();

        expanded.Compile()(3).Should().BeTrue();
        expanded.Compile()(1).Should().BeFalse();
    }

    [Fact]
    public void Expand_CompileOnExpressionVariable_IsStillExpanded()
    {
        Expression<Func<int, bool>> inner = x => x > 2;
        Expression<Func<int, bool>> expr = x => inner.Compile()(x);

        var expanded = expr.Expand();

        expanded.ToString().Should().NotContain("Compile");
        expanded.Compile()(3).Should().BeTrue();
    }
}
