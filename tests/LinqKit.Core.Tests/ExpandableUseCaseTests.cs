using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using LinqKit;
using LinqKit.Core;
using LinqKitCoreTests.Model;
using Xunit;

namespace LinqKitCoreTests;

/// <summary>
/// The AsExpandable / Invoke / Expand use cases of the README and of typical business-rule queries,
/// run over a <see cref="StrictQueryable{T}"/> so that a leftover Invoke or expression reference fails the test.
/// </summary>
public class ExpandableUseCaseTests
{
    private static readonly Expression<Func<Purchase, bool>> IsBigPurchase = p => p.Price > 1000;

    [Fact]
    public void StrictQueryable_Rejects_Invoke_Without_AsExpandable()
    {
        var purchases = Shop.Purchases().AsStrictQueryable();

        Action act = () => purchases.Where(p => IsBigPurchase.Invoke(p)).ToList();

        act.Should().Throw<NotSupportedException>();
        purchases.AsExpandable().Where(p => IsBigPurchase.Invoke(p)).Select(p => p.Id).ToList().Should().Equal(10);
    }

    [Fact]
    public void Compile_On_A_Navigation_Collection_Is_Replaced_By_The_Expression()
    {
        var customers = Shop.Customers().AsStrictQueryable();
        var criteria = IsBigPurchase;

        Action act = () => customers.Where(c => c.Purchases.Any(criteria.Compile())).ToList();
        var names = customers.AsExpandable().Where(c => c.Purchases.Any(criteria.Compile())).Select(c => c.Name).ToList();

        act.Should().Throw<NotSupportedException>();
        names.Should().Equal("Alice");
    }

    [Fact]
    public void Expression_Variable_In_A_Let_Subquery_Is_Inlined()
    {
        var customers = Shop.Customers().AsStrictQueryable();
        var purchases = Shop.Purchases().AsQueryable();
        var criteria = IsBigPurchase;

        IQueryable<string> Query(IQueryable<Customer> source) =>
            from c in source
            let custPurchases = purchases.Where(p => p.CustomerId == c.Id)
            where custPurchases.Any(criteria)
            select c.Name;

        Action act = () => Query(customers).ToList();

        act.Should().Throw<NotSupportedException>();
        Query(customers.AsExpandable()).ToList().Should().Equal("Alice");
    }

    [Fact]
    public void Expand_Removes_Invoke_From_A_Combined_Expression()
    {
        Expression<Func<Purchase, bool>> criteria1 = p => p.Price > 1000;
        Expression<Func<Purchase, bool>> criteria2 = p => criteria1.Invoke(p) || p.Description.Contains("a");
        var purchases = Shop.Purchases().AsStrictQueryable();

        var expanded = criteria2.Expand();

        expanded.ToString().Should().Be("p => ((p.Price > 1000) OrElse p.Description.Contains(\"a\"))");
        purchases.Where(expanded).Select(p => p.Id).ToList().Should().Equal(10, 13);
        purchases.AsExpandable().Where(criteria2).Select(p => p.Id).ToList().Should().Equal(10, 13);
    }

    [Fact]
    public void Invoked_Aggregate_Over_A_Group()
    {
        Expression<Func<IQueryable<Purchase>, decimal?>> averagePrice = ps => ps.Average(p => (decimal?)p.Price);
        var purchases = Shop.Purchases().AsStrictQueryable();

        var result =
            (from p in purchases.AsExpandable()
             group p by p.Date into g
             select new { Date = g.Key, AveragePrice = averagePrice.Invoke(g.AsQueryable()) })
            .ToList();

        result.Should().Equal(
            [
                new { Date = new DateTime(2026, 1, 1), AveragePrice = (decimal?)760m },
                new { Date = new DateTime(2026, 1, 2), AveragePrice = (decimal?)275m },
                new { Date = new DateTime(2026, 1, 3), AveragePrice = (decimal?)300m }
            ]);
    }

    [Fact]
    public void Nested_PredicateBuilder_Predicates()
    {
        var inner = PredicateBuilder.New<Purchase>();
        inner = inner.Start(p => p.Description.Contains("o"));
        inner = inner.Or(p => p.Description.Contains("e"));

        var outer = PredicateBuilder.New<Purchase>();
        outer = outer.Start(p => p.Price > 100);
        outer = outer.And(p => p.Price < 1000);
        outer = outer.And(inner);

        var purchases = Shop.Purchases().AsStrictQueryable();

        purchases.AsExpandable().Where(outer).Select(p => p.Id).ToList().Should().Equal(12, 14);
    }

    [Fact]
    public void Chain_Of_Invoked_Rules_With_One_Two_Arguments_In_A_Let()
    {
        Expression<Func<Customer, Purchase, bool>> sameCountry = (c, p) => c.Country == p.Country;
        Expression<Func<Customer, Purchase, bool>> sameCurrency = (c, p) => c.Currency == p.Currency;
        Expression<Func<Purchase, bool>> hasQuantity = p => p.Quantity > 0;
        var customers = Shop.Customers().AsStrictQueryable();
        var purchases = Shop.Purchases().AsQueryable();

        var result =
            (from c in customers.AsExpandable()
             from p in purchases.AsExpandable()
             where p.CustomerId == c.Id
             let error =
                   !sameCountry.Invoke(c, p) ? "country"
                 : !sameCurrency.Invoke(c, p) ? "currency"
                 : !hasQuantity.Invoke(p) ? "quantity"
                 : "ok"
             select new { p.Id, error })
            .ToList();

        result.Should().Equal(
            [
                new { Id = 10, error = "ok" },
                new { Id = 11, error = "ok" },
                new { Id = 12, error = "currency" },
                new { Id = 13, error = "country" },
                new { Id = 14, error = "ok" }
            ]);
    }

    [Theory]
    [InlineData(true, new[] { 10, 13, 14 })]
    [InlineData(false, new[] { 10, 11, 12, 13, 14 })]
    public void Conditionally_Chosen_Cross_Join_With_An_Invoked_Join_Condition(bool webOnly, int[] expected)
    {
        Expression<Func<Customer, Purchase, bool>> belongsTo = (c, p) => c.Id == p.CustomerId;
        var customers = Shop.Customers().AsStrictQueryable();
        var purchases = Shop.Purchases().AsStrictQueryable();

        var query = webOnly
            ? from c in customers.AsExpandable()
              from p in purchases.AsExpandable()
              where belongsTo.Invoke(c, p) && p.Channel == SalesChannel.Web
              select p.Id
            : from c in customers.AsExpandable()
              from p in purchases.AsExpandable()
              where belongsTo.Invoke(c, p)
              select p.Id;

        query.ToList().Should().Equal(expected);
    }

    [Fact]
    public void Invoke_With_A_Captured_Local_As_Argument()
    {
        Expression<Func<Purchase, SalesChannel, bool>> soldVia = (p, channel) => p.Channel == channel;
        var store = SalesChannel.Store;
        var purchases = Shop.Purchases().AsStrictQueryable();

        purchases.AsExpandable().Where(p => soldVia.Invoke(p, store)).Select(p => p.Id).ToList().Should().Equal(11, 12);
    }

    [Theory]
    [InlineData(false, new[] { "10/100/2", "11/101/1", "11/102/1" })]
    [InlineData(true, new[] { "10/100/1", "11/101/1", "11/102/1", "14/103/1" })]
    public void Hand_Built_Three_Argument_Priority_Expression_Invoked_In_A_Let(bool ignoreRules, string[] expected)
    {
        var rules = ShipmentPriority();
        var purchases = Shop.Purchases().AsStrictQueryable();
        var customers = Shop.Customers().AsQueryable();
        var shipments = Shop.Shipments().AsQueryable();

        var result =
            (from p in purchases.AsExpandable()
             join c in customers on p.CustomerId equals c.Id
             join s in shipments on p.Id equals s.PurchaseId
             let priority = ignoreRules ? 1 : rules.Invoke(c, p, s)
             where priority > 0
             select p.Id + "/" + s.Id + "/" + priority)
            .ToList();

        result.Should().Equal(expected);
    }

    /// <summary>
    /// (c, p, s) => c.Country == p.Country &amp;&amp; p.Quantity == s.Quantity ? 2 : p.Price &lt; 100 ? 1 : 0,
    /// built node by node the way rule tables read from configuration are.
    /// </summary>
    private static Expression<Func<Customer, Purchase, Shipment, int>> ShipmentPriority()
    {
        var c = Expression.Parameter(typeof(Customer), "c");
        var p = Expression.Parameter(typeof(Purchase), "p");
        var s = Expression.Parameter(typeof(Shipment), "s");

        var rulesByPriority = new (Expression Condition, int Priority)[]
        {
            (Expression.AndAlso(
                Expression.Equal(Expression.Property(c, nameof(Customer.Country)), Expression.Property(p, nameof(Purchase.Country))),
                Expression.Equal(Expression.Property(p, nameof(Purchase.Quantity)), Expression.Property(s, nameof(Shipment.Quantity)))), 2),
            (Expression.LessThan(Expression.Property(p, nameof(Purchase.Price)), Expression.Constant(100m)), 1)
        };

        var body = Enumerable.Reverse(rulesByPriority)
            .Aggregate((Expression)Expression.Constant(0), (otherwise, rule) => Expression.Condition(rule.Condition, Expression.Constant(rule.Priority), otherwise));

        return Expression.Lambda<Func<Customer, Purchase, Shipment, int>>(body, c, p, s);
    }

    [Fact]
    public void Left_Join_With_A_Let_Subquery_And_An_Invoked_Calculation()
    {
        Expression<Func<Purchase, int, int>> openQuantity = (p, shipped) => p.Quantity - shipped;
        var purchases = Shop.Purchases().AsStrictQueryable();
        var customers = Shop.Customers().AsQueryable();
        var shipments = Shop.Shipments().AsQueryable();

        var result =
            (from p in purchases.AsExpandable()
             let shipped = from s in shipments where s.PurchaseId == p.Id select s.Quantity
             join c in customers on p.CustomerId equals c.Id into cs
             from c in cs.DefaultIfEmpty()
             let open = openQuantity.Invoke(p, shipped.Sum())
             where open > 0
             select new { p.Id, Customer = c == null ? null : c.Name, open })
            .ToList();

        result.Should().Equal(
            [
                new { Id = 12, Customer = "Bob", open = 1 },
                new { Id = 14, Customer = "Carol", open = 1 }
            ]);
    }

    [Fact]
    public void Optional_Filters_Combined_With_Invoke_Then_Expanded()
    {
        Expression<Func<Purchase, bool>> permission = p => p.Country == "FI";
        Expression<Func<Purchase, bool>> filter = p => p.Price > 100;
        Expression<Func<Purchase, bool>> secondaryFilter = null;
        var purchases = Shop.Purchases().AsStrictQueryable();

        var combined = AllOf(permission, filter, secondaryFilter);

        combined.ToString().Should().NotContain("Invoke");
        purchases.Where(combined).Select(p => p.Id).ToList().Should().Equal(10, 14);
        purchases.AsExpandable().Where(combined).Select(p => p.Id).ToList().Should().Equal(10, 14);
    }

    private static Expression<Func<T, bool>> AllOf<T>(params Expression<Func<T, bool>>[] filters) =>
        filters
            .Where(f => f != null)
            .Aggregate((Expression<Func<T, bool>>)(x => true), (all, next) => x => all.Invoke(x) && next.Invoke(x))
            .Expand();

    [Fact]
    public void AsExpandable_After_OrderBy_Keeps_The_Order_For_Paging()
    {
        Expression<Func<Purchase, string>> label = p => p.Description + " x" + p.Quantity;
        var purchases = Shop.Purchases().AsStrictQueryable();

        var page = purchases
            .OrderBy(p => p.Price)
            .AsExpandable()
            .Skip(1)
            .Take(2)
            .Select(p => label.Invoke(p))
            .ToList();

        page.Should().Equal("chair x0", "monitor x2");
    }

    [Fact]
    public void Terminal_Operators_Expand_Before_Execute()
    {
        Expression<Func<Purchase, bool>> hasQuantity = p => p.Quantity > 0;
        Expression<Func<Purchase, decimal>> lineTotal = p => p.Price * p.Quantity;
        var purchases = Shop.Purchases().AsStrictQueryable().AsExpandable();

        (from p in purchases where p.CustomerId == 1 let ok = hasQuantity.Invoke(p) select ok).All(ok => ok).Should().BeTrue();
        (from p in purchases where p.CustomerId == 2 let ok = hasQuantity.Invoke(p) select ok).All(ok => ok).Should().BeFalse();
        purchases.Count(p => hasQuantity.Invoke(p)).Should().Be(4);
        purchases.Sum(p => lineTotal.Invoke(p)).Should().Be(2540m);
        purchases.Any(p => IsBigPurchase.Invoke(p)).Should().BeTrue();
        purchases.First(p => !hasQuantity.Invoke(p)).Id.Should().Be(13);
    }

    /// <summary>Untyped callers, such as dynamic LINQ, go through the non-generic IQueryProvider.CreateQuery.</summary>
    private static IQueryable UntypedWhere(IQueryable<Purchase> purchases, Expression<Func<Purchase, bool>> predicate) =>
        purchases.Provider.CreateQuery(
            Expression.Call(typeof(Queryable), nameof(Queryable.Where), [typeof(Purchase)], purchases.Expression, Expression.Quote(predicate)));

    [Fact]
    public void Untyped_CreateQuery_Expands()
    {
        var purchases = Shop.Purchases().AsStrictQueryable().AsExpandable();

        var query = UntypedWhere(purchases, p => IsBigPurchase.Invoke(p));

        query.Cast<Purchase>().Select(p => p.Id).ToList().Should().Equal(10);
    }

    [Fact(Skip = "Known gap: ExpandableQueryProvider's non-generic CreateQuery returns the inner query, not an expandable one.")]
    public void Untyped_CreateQuery_Stays_Expandable()
    {
        var purchases = Shop.Purchases().AsStrictQueryable().AsExpandable();

        var query = UntypedWhere(purchases, p => p.Quantity > 0).Cast<Purchase>();

        query.Where(p => IsBigPurchase.Invoke(p)).Select(p => p.Id).ToList().Should().Equal(10);
    }

    [Fact]
    public void Default_Expression_For_A_Missing_Rule()
    {
        Expression<Func<Purchase, bool>> configuredRule = null;
        var rule = configuredRule ?? (p => true);
        var purchases = Shop.Purchases().AsStrictQueryable();

        purchases.AsExpandable().Where(p => rule.Invoke(p) && p.Quantity > 0).Select(p => p.Id).ToList().Should().Equal(10, 11, 12, 14);
    }

    [Fact]
    public void Hand_Built_Or_Chain_Of_Key_Pairs_Expands_And_Joins()
    {
        var allowed = AnyPair((1, 10), (2, 13), (3, 10));
        var customers = Shop.Customers().AsStrictQueryable();
        var purchases = Shop.Purchases().AsQueryable();

        var pairs =
            (from c in customers.AsExpandable()
             from p in purchases
             where allowed.Invoke(c, p)
             select new { CustomerId = c.Id, PurchaseId = p.Id })
            .ToList();

        allowed.Expand().ToString().Should().Contain("((c.Id == 1) AndAlso (p.Id == 10))");
        pairs.Should().Equal(
            [
                new { CustomerId = 1, PurchaseId = 10 },
                new { CustomerId = 2, PurchaseId = 13 },
                new { CustomerId = 3, PurchaseId = 10 }
            ]);
    }

    private static Expression<Func<Customer, Purchase, bool>> AnyPair(params (int CustomerId, int PurchaseId)[] keys)
    {
        var c = Expression.Parameter(typeof(Customer), "c");
        var p = Expression.Parameter(typeof(Purchase), "p");

        var body = keys
            .Select(k => (Expression)Expression.AndAlso(
                Expression.Equal(Expression.Property(c, nameof(Customer.Id)), Expression.Constant(k.CustomerId)),
                Expression.Equal(Expression.Property(p, nameof(Purchase.Id)), Expression.Constant(k.PurchaseId))))
            .Aggregate(Expression.OrElse);

        return Expression.Lambda<Func<Customer, Purchase, bool>>(body, c, p);
    }

    [Fact]
    public void AsExpandable_Is_Idempotent_And_Survives_Query_Operators()
    {
        var purchases = Shop.Purchases().AsStrictQueryable().AsExpandable();

        purchases.AsExpandable().Should().BeSameAs(purchases);
        var filtered = purchases.Where(p => IsBigPurchase.Invoke(p));
        filtered.AsExpandable().Should().BeSameAs(filtered);
    }
}
