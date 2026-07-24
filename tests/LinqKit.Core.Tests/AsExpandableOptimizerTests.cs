using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using LinqKit;
using LinqKit.Core;
using Xunit;

namespace LinqKitCoreTests;

public class AsExpandableOptimizerTests
{
    private readonly IQueryable<int> _source = new[] { 1, 2, 3, 4 }.AsQueryable();

    [Fact]
    public void CustomOptimizer_IsUsed_AfterQueryComposition()
    {
        var optimizerCalls = 0;
        Func<Expression, Expression> optimizer = e =>
        {
            optimizerCalls++;
            return e;
        };

        // One optimizer call for the .Where composition; enumeration uses the already-optimized inner query.
        var result = _source.AsExpandable(optimizer).Where(x => x > 2).ToList();

        result.Should().BeEquivalentTo(new[] { 3, 4 });
        optimizerCalls.Should().Be(1);
    }

    [Fact]
    public void CustomOptimizer_IsUsed_ForExecutionAfterQueryComposition()
    {
        var optimizerCalls = 0;
        Func<Expression, Expression> optimizer = e =>
        {
            optimizerCalls++;
            return e;
        };

        // One optimizer call for the .Where composition, one for the Count() execution
        // (which runs on the wrapper created by that composition).
        var count = _source.AsExpandable(optimizer).Where(x => x > 2).Count();

        count.Should().Be(2);
        optimizerCalls.Should().Be(2);
    }

    [Fact]
    public void AsExpandable_WithDifferentOptimizer_RewrapsQuery()
    {
        var optimizerCalls = 0;
        Func<Expression, Expression> optimizer = e =>
        {
            optimizerCalls++;
            return e;
        };

        var query = _source.AsExpandable().AsExpandable(optimizer);

        query.Where(x => x > 2).ToList();

        optimizerCalls.Should().BeGreaterThan(0);
    }

    [Fact]
    public void AsExpandable_WithoutOptimizer_OnWrappedQuery_ReturnsSameInstance()
    {
        var query = _source.AsExpandable();

        query.AsExpandable().Should().BeSameAs(query);
    }
}
