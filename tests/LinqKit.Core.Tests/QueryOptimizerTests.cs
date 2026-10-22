using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using LinqKit;
using LinqKit.Core;
using LinqKitCoreTests.Model;
using Xunit;

namespace LinqKitCoreTests;

/// <summary>
/// The README's "More optimized queries!" use cases with Linq.Expression.Optimizer, checked on the expression
/// tree the underlying provider receives.
/// </summary>
public class QueryOptimizerTests
{
    internal static IQueryable<int> ReadmeQuery(IQueryable<Purchase> purchases, int t)
    {
        var qry1 =
            from o in purchases
            let myTemp =
                t == 1 ? o.Price + 10 :
                t == 2 ? o.Price - 10 :
                o.Price
            select new
            {
                o.Id,
                FixedAmount = myTemp
            };

        return
            from x in qry1
            where x.FixedAmount < 300
            select x.Id;
    }

    internal static List<Expression> Nodes(Expression expression)
    {
        var collector = new NodeCollector();
        collector.Visit(expression);
        return collector.Nodes;
    }

    /// <summary>Member accesses on compiler-generated closures, that is, captured local variables.</summary>
    internal static IEnumerable<MemberExpression> CapturedVariables(Expression expression) =>
        Nodes(expression).OfType<MemberExpression>().Where(m => m.Expression is ConstantExpression c && c.Type.Name.StartsWith("<>"));

    private sealed class NodeCollector : ExpressionVisitor
    {
        public List<Expression> Nodes { get; } = new();

        public override Expression Visit(Expression node)
        {
            if (node != null)
            {
                Nodes.Add(node);
            }

            return base.Visit(node);
        }
    }

    [Fact]
    public void Without_Optimizer_The_Provider_Gets_The_Conditional_And_The_Captured_Variable()
    {
        var purchases = Shop.Purchases().AsStrictQueryable();

        var ids = ReadmeQuery(purchases.AsExpandable(), 2).ToList();

        ids.Should().Equal(11, 13, 14);
        var executed = purchases.StrictProvider.LastExecuted;
        Nodes(executed).OfType<ConditionalExpression>().Should().HaveCount(2);
        CapturedVariables(executed).Should().Contain(m => m.Member.Name == "t");
    }

    [Fact]
    public void Dynamic_Option_Removes_The_Conditional_And_The_Captured_Variable()
    {
        var purchases = Shop.Purchases().AsStrictQueryable();

        var ids = ReadmeQuery(purchases.AsExpandable(ExpressionOptimizer.visit), 2).ToList();

        ids.Should().Equal(11, 13, 14);
        var executed = purchases.StrictProvider.LastExecuted;
        Nodes(executed).OfType<ConditionalExpression>().Should().BeEmpty();
        CapturedVariables(executed).Should().BeEmpty();
        Nodes(executed).Should().Contain(n => n.NodeType == ExpressionType.Subtract);
        Nodes(executed).Should().NotContain(n => n.NodeType == ExpressionType.Add);
    }

    [Theory]
    [InlineData(0, new[] { 11, 13 })]
    [InlineData(1, new[] { 11, 13 })]
    [InlineData(2, new[] { 11, 13, 14 })]
    public void Dynamic_Option_Gives_The_Same_Results_As_No_Optimizer(int t, int[] expected)
    {
        var plain = ReadmeQuery(Shop.Purchases().AsStrictQueryable().AsExpandable(), t).ToList();
        var optimized = ReadmeQuery(Shop.Purchases().AsStrictQueryable().AsExpandable(ExpressionOptimizer.visit), t).ToList();

        plain.Should().Equal(expected);
        optimized.Should().Equal(expected);
    }

    [Fact]
    public void Optimizer_Folds_A_Captured_Argument_Of_An_Invoked_Expression()
    {
        Expression<Func<Purchase, int, bool>> atLeastTwice = (p, minimum) => p.Quantity >= minimum * 2;
        var minimum = 1;
        var purchases = Shop.Purchases().AsStrictQueryable();

        var ids = purchases.AsExpandable(ExpressionOptimizer.visit).Where(p => atLeastTwice.Invoke(p, minimum)).Select(p => p.Id).ToList();

        ids.Should().Equal(11, 14);
        var executed = purchases.StrictProvider.LastExecuted;
        Nodes(executed).Should().NotContain(n => n.NodeType == ExpressionType.Multiply);
        CapturedVariables(executed).Should().BeEmpty();
        Nodes(executed).OfType<ConstantExpression>().Should().Contain(c => Equals(c.Value, 2));
    }

    /// <remarks>The optimizer evaluates captured variables of primitive types only; a captured string stays a parameter.</remarks>
    [Theory]
    [InlineData(false, 0, new[] { 10, 11, 12, 13, 14 })]
    [InlineData(true, 1, new[] { 10, 11, 12, 14 })]
    public void Optimizer_Removes_An_Optional_Filter_Switch(bool useMinimum, int minimum, int[] expected)
    {
        var purchases = Shop.Purchases().AsStrictQueryable();

        var ids = purchases.AsExpandable(ExpressionOptimizer.visit).Where(p => !useMinimum || p.Quantity >= minimum).Select(p => p.Id).ToList();

        ids.Should().Equal(expected);
        var executed = purchases.StrictProvider.LastExecuted;
        Nodes(executed).Should().NotContain(n => n.NodeType == ExpressionType.OrElse);
        CapturedVariables(executed).Should().BeEmpty();
    }

    [Fact]
    public void Optimizer_Runs_On_Terminal_Operators()
    {
        var t = 2;
        var purchases = Shop.Purchases().AsStrictQueryable();

        var count = purchases.AsExpandable(ExpressionOptimizer.visit).Count(p => (t == 1 ? p.Price + 10 : p.Price - 10) < 300);

        count.Should().Be(3);
        Nodes(purchases.StrictProvider.LastExecuted).OfType<ConditionalExpression>().Should().BeEmpty();
    }

    [Fact]
    public void Optimizer_Runs_On_Untyped_CreateQuery()
    {
        var t = 2;
        Expression<Func<Purchase, bool>> predicate = p => (t == 1 ? p.Price + 10 : p.Price - 10) < 300;
        var purchases = Shop.Purchases().AsStrictQueryable();
        var expandable = purchases.AsExpandable(ExpressionOptimizer.visit);

        var query = expandable.Provider.CreateQuery(
            Expression.Call(typeof(Queryable), nameof(Queryable.Where), [typeof(Purchase)], expandable.Expression, Expression.Quote(predicate)));

        query.Cast<Purchase>().Select(p => p.Id).ToList().Should().Equal(11, 13, 14);
        Nodes(purchases.StrictProvider.LastExecuted).OfType<ConditionalExpression>().Should().BeEmpty();
    }
}

[CollectionDefinition(Name, DisableParallelization = true)]
public class StaticQueryOptimizerCollection
{
    public const string Name = "LinqKitExtension.QueryOptimizer";
}

/// <summary>Sets the process-wide LinqKitExtension.QueryOptimizer, so it must not run beside other tests.</summary>
[Collection(StaticQueryOptimizerCollection.Name)]
public class StaticQueryOptimizerTests
{
    [Fact]
    public void Static_Option_Optimizes_Every_AsExpandable()
    {
        var original = LinqKitExtension.QueryOptimizer;
        LinqKitExtension.QueryOptimizer = ExpressionOptimizer.visit;
        try
        {
            var purchases = Shop.Purchases().AsStrictQueryable();

            var ids = QueryOptimizerTests.ReadmeQuery(purchases.AsExpandable(), 2).ToList();

            ids.Should().Equal(11, 13, 14);
            var executed = purchases.StrictProvider.LastExecuted;
            QueryOptimizerTests.Nodes(executed).OfType<ConditionalExpression>().Should().BeEmpty();
            QueryOptimizerTests.CapturedVariables(executed).Should().BeEmpty();
        }
        finally
        {
            LinqKitExtension.QueryOptimizer = original;
        }
    }
}
