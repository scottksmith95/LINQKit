using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using LinqKit;
using Xunit;

namespace LinqKitCoreTests;

public class ExpressionStarterNotTests
{
    private readonly List<string> _list = new() { "a", "b", "c" };

    [Fact]
    public void Not_OnUnstartedPredicateWithDefaultFalse_ReturnsTrue()
    {
        var predicate = PredicateBuilder.New<string>(false);

        predicate.Not();

        _list.Where(predicate).Should().BeEquivalentTo(_list);
    }

    [Fact]
    public void Not_OnUnstartedPredicateWithDefaultTrue_ReturnsFalse()
    {
        var predicate = PredicateBuilder.New<string>(true);

        predicate.Not();

        _list.Where(predicate).Should().BeEmpty();
    }

    [Fact]
    public void Not_OnStartedPredicate_NegatesPredicate()
    {
        var predicate = PredicateBuilder.New<string>(s => s == "a");

        predicate.Not();

        _list.Where(predicate).Should().BeEquivalentTo("b", "c");
    }
}
