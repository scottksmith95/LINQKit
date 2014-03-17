What is LINQKit?
=======

LINQKit is a free set of extensions for LINQ to SQL and Entity Framework power users. It comprises the following:

* An extensible implementation of AsExpandable()
* A public expression visitor base class (ExpressionVisitor)
* PredicateBuilder
* Linq.Expr and Linq.Func shortcut methods

With LINQKit, you can:

* Plug expressions into EntitySets and EntityCollections
* Use expression variables in subqueries
* Combine expressions (have one expression call another)
* Dynamically build predicates
* Leverage AsExpandable to add your own extensions.

AsExpandable is based on a [very clever](http://tomasp.net/blog/linq-expand.aspx) project by Tomas Petricek. ExpressionVisitor comes from a [sample by Matt Warren](http://blogs.msdn.com/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx).

Plugging Expressions into EntitySets / EntityCollections: The Problem
=======

We'll assume two very simple LINQ to SQL entities: Customer and Purchase, in a one-to-many relationship. (Entity Framework works in exactly the same way in these examples).

Suppose we wanted to write a method called QueryCustomers that returned the names of all customers who'd made at least one purchase that satisfied a particular criteria.  So, if we wanted all customers who'd made a purchase over $1000, we'd call this hypothetical QueryCustomer method as follows:

```csharp
string[] bigSpenders = QueryCustomers (p => p.Price > 1000);
```

Here's what the method's signature might look like:

```csharp
static string[] QueryCustomers (some-type purchaseCriteria)
{
   ...
}
```

Because we're querying a database, some-type must be Expression<Func<>> rather than just Func<>. This ensures our query will end up as an expression tree that LINQ to SQL or Entity Framework can traverse and convert to a SQL statement. In other words:

```csharp
static string[] QueryCustomers (Expression<Func<Purchase,bool>> purchaseCriteria)
{
   ...
}
```

We chose Expression<Func<Purchase,bool>> because our pluggable criteria will accept a Purchase object and return true or false, depending on whether or not to include that Purchase object. Here's how we might write the whole method:

```csharp
static string[] QueryCustomers (Expression<Func<Purchase, bool>> purchaseCriteria)
{
  var data = new MyDataContext();    // or MyObjectContext()

  var query =
    from c in data.Customers
    where c.Purchases.Any (purchaseCriteria)  // will not compile
    select c.Name;

  return query.ToArray();
}
```

But there's a problem: Customer.Purchases is of type EntitySet<> (or EntityCollection<> with EF) neither of which implements IQueryable<>. This means that we can't call Queryable's Any method (the one that accepts an Expression<Func<>>) and our query won't compile!

It would be a different story if we were querying the Purchases table directly (rather the via the Customer.Purchases association property). The Purchases property is of type Table<Purchase> which implements IQueryable, allowing us to do the following:

```csharp
bool any = data.Purchases.Any (purchaseCriteria);
```

Of course, we could rewrite QueryCustomers to accept a Func<Purchase,bool> instead:

```csharp
static string[] QueryCustomers (Func<Purchase,bool> purchaseCriteria)
...
```

Everything would then compile, but LINQ to SQL or Entity Framework would throw an exception because it wouldn't be able to understand what was inside the Func delegate. And fair enough too: the query pipeline would have to disassemble IL code to work around that!

Plugging Expressions into EntitySets / EntityCollections: The Solution
=======

Here's how to solve the above problem with LINQKit:

1. Call AsExpandable() on the Table<> object
2. Call Compile() on the expression variable, when used on an EntitySet or EntityCollection.
 
```csharp
static string[] QueryCustomers (Expression<Func<Purchase, bool>> purchaseCriteria)
{
  var data = new MyDataContext();

  var query =
    from c in data.Customers.AsExpandable()
    where c.Purchases.Any (purchaseCriteria.Compile())
    select c.Name;

  return query.ToArray();
}
````

Compile is an inbuilt method in the Expression class. It converts the Expression<Func<Purchase,bool> into a plain Func<Purchase,bool> which satisfies the compiler. Of course, if this method actually ran, we'd end up with compiled IL code instead of an expression tree, and LINQ to SQL or Entity Framework would throw an exception. But here's the clever part: Compile never actually runs; nor does LINQ to SQL or Entity Framework ever get to see it. The call to Compile gets stripped out entirely by a special wrapper that was created by calling AsExpandable, and substituted for a correct expression tree.

You can find out more about how AsExpandable works in [Tomas Petricek's blog](http://tomasp.net/blog/linq-expand.aspx).

Using Expression Variables in Subqueries
=======

Suppose we want to write our previous example without using the Customer.Purchases association property. (This might happen in real life if querying an ad-hoc relationship.) To recap, our query is to retrieve the names of all customers who have had made at least one purchase satisfying a particular criteria. Here's how we might proceed:

```csharp
static string[] QueryCustomers (Expression<Func<Purchase, bool>> purchaseCriteria)
{
  var data = new MyDataContext();

  var query =
    from c in data.Customers
    let custPurchases = data.Purchases.Where (p => p.CustomerID == c.ID)
    where custPurchases.Any (purchaseCriteria)
    select c.Name;

  return query.ToArray();
}
```

Seem reasonable enough? Entity Framework handles this query without error but LINQ to SQL throws an exception:

    Unsupported overload used for query operator 'Any'.

The problem is that LINQ to SQL cannot handle references to expressions (such as purchaseCriteria) within subqueries. "But where is the subquery," you might ask! The answer lies in the compiler: C# generates a subquery when it translates the let clause into lambda/method syntax.

The solution, with LINQKit, is simply to call AsExpandable() on the first table in the query:

```csharp
static string[] QueryCustomers (Expression<Func<Purchase, bool>> purchaseCriteria)
{
  var data = new MyDataContext();

  var query =
    from c in data.Customers.AsExpandable()
    let custPurchases = data.Purchases.Where (p => p.CustomerID == c.ID)
    where custPurchases.Any (purchaseCriteria)
    select c.Name;

  return query.ToArray();
}
```

Nothing else needs to be changed. The wrapper that AsExpandable generates looks specifically for references to expressions, and substitutes the expression in place of the reference. Voila!

Combining Expressions
=======

The AsExpandable wrapper also lets you write expressions that call other expressions. All you need to do is:

1. Call Invoke to call the inner expression
2. Call Expand on the final result.

For example:

```csharp
Expression<Func<Purchase,bool>> criteria1 = p => p.Price > 1000;
Expression<Func<Purchase,bool>> criteria2 = p => criteria1.Invoke (p) || p.Description.Contains ("a");

Console.WriteLine (criteria2.Expand().ToString());
```

(Invoke and Expand are extension methods in LINQKit.) Here's the output:

```csharp
p => ((p.Price > 1000) || p.Description.Contains("a"))
```

Notice that we have a nice clean expression: the call to Invoke has been stripped away.

If you're using an Invoked expression within a LINQ to SQL or Entity Framework query, and have called AsExpandable on the Table, you can optionally skip step 2. This is because AsExpandable automatically calls Expand on expressions. This means either of the following is valid:

```csharp
var query = data.Purchases.AsExpandable().Where (criteria2);
```

```csharp
var query = data.Purchases.Where (criteria2.Expand());
```

Be sure to remember that AsExpandable() works on IQueryable<T> and Expand() works on Expression<TDelegate>

The one thing to watch is recursive expressions: these cannot be Expanded! Recursive expressions usually happen by accident when you reuse a variable. It's an easy mistake to make:

```csharp
Expression<Func<Purchase,bool>> criteria = p => p.Price > 1000;
criteria = p => criteria.Invoke (p) || p.Description.Contains ("a");
```

That last line recursively calls itself and the original predicate (p.Price>1000) is lost!

PredicateBuilder
=======

Click [here](http://www.albahari.com/nutshell/predicatebuilder.aspx) for information on how to use PredicateBuilder.

When applying expressions built with PredicateBuilder to an Entity Framework query, remember to call AsExpandable on the first table in the query.

Original source and author
=======

http://www.albahari.com/nutshell/linqkit.aspx

Permission has been granted to have this repo be the official source for this project.

License
=======
LINQKit is free. The source code is issued under a permissive free license, which means you can modify it as you please, and incorporate it into your own commercial or non-commercial software.
