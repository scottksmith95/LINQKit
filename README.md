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

If you like LINQKit, consider voting for it on the Entity Framework UserVoice page (http://data.uservoice.com/forums/72025-entity-framework-feature-suggestions/suggestions/2390752-add-support-for-dynamically-extended-queries-as-pe). 

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

When applying expressions built with PredicateBuilder to an Entity Framework query, remember to call AsExpandable on the first table in the query.

##Dynamically Composing Expression Predicates

Suppose you want to write a LINQ to SQL or Entity Framework query that implements a keyword-style search. In other words, a query that returns rows whose description contains some or all of a given set of keywords.

We can proceed as follows:

```csharp
IQueryable<Product> SearchProducts (params string[] keywords)
{
  IQueryable<Product> query = dataContext.Products;

  foreach (string keyword in keywords)
  {
    string temp = keyword;
    query = query.Where (p => p.Description.Contains (temp));
  }
  return query;
}
```

The temporary variable in the loop is required to avoid the outer variable trap, where the same variable is captured for each iteration of the foreach loop.

So far, so good. But this only handles the case where you want to match all of the specified keywords. Suppose instead, we wanted products whose description contains any of the supplied keywords. Our previous approach of chaining Where operators is completely useless! We could instead chain Union operators, but this would be inefficient. The ideal approach is to dynamically construct a lambda expression tree that performs an or-based predicate.

Of all the things that will drive you to manually constructing expression trees, the need for dynamic predicates is the most common in a typical business application. Fortunately, it’s possible to write a set of simple and reusable extension methods that radically simplify this task. This is the role of our PredicateBuilder class.

##Using PredicateBuilder

Here's how to solve the preceding example with PredicateBuilder:

```csharp
IQueryable<Product> SearchProducts (params string[] keywords)
{
  var predicate = PredicateBuilder.False<Product>();

  foreach (string keyword in keywords)
  {
    string temp = keyword;
    predicate = predicate.Or (p => p.Description.Contains (temp));
  }
  return dataContext.Products.Where (predicate);
}
```

If querying with Entity Framework, change the last line to this:

```csharp
return objectContext.Products.AsExpandable().Where (predicate);
```

##How it Works

The True and False methods do nothing special: they are simply convenient shortcuts for creating an Expression<Func<T,bool>> that initially evaluates to true or false. So the following:

```csharp
var predicate = PredicateBuilder.True <Product> ();
```

is just a shortcut for this:

```csharp
Expression<Func<Product, bool>> predicate = c => true;
```

When you’re building a predicate by repeatedly stacking and/or conditions, it’s useful to have a starting point of either true or false (respectively). Our SearchProducts method still works if no keywords are supplied.

The interesting work takes place inside the And and Or methods. We start by invoking the second expression with the first expression’s parameters. An Invoke expression calls another lambda expression using the given expressions as arguments. We can create the conditional expression from the body of the first expression and the invoked version of the second. The final step is to wrap this in a new lambda expression.

Entity Framework's query processing pipeline cannot handle invocation expressions, which is why you need to call AsExpandable on the first object in the query. By calling AsExpandable, you activate LINQKit's expression visitor class which substitutes invocation expressions with simpler constructs that Entity Framework can understand.

##More Examples

A useful pattern in writing a data access layer is to create a reusable predicate library. Your queries, then, consist largely of select and orderby clauses, the filtering logic farmed out to your library. Here's a simple example:

```csharp
public partial class Product
{
  public static Expression<Func<Product, bool>> IsSelling()
  {
    return p => !p.Discontinued && p.LastSale > DateTime.Now.AddDays (-30);
  }
}
```

We can extend this by adding a method that uses PredicateBuilder:

```csharp
public partial class Product
{
  public static Expression<Func<Product, bool>> ContainsInDescription (
                                                params string[] keywords)
  {
    var predicate = PredicateBuilder.False<Product>();
    foreach (string keyword in keywords)
    {
      string temp = keyword;
      predicate = predicate.Or (p => p.Description.Contains (temp));
    }
    return predicate;
  }
}
```

This offers an excellent balance of simplicity and reusability, as well as separating business logic from expression plumbing logic. To retrieve all products whose description contains “BlackBerry” or “iPhone”, along with the Nokias and Ericssons that are selling, you would do this:

```csharp
var newKids  = Product.ContainsInDescription ("BlackBerry", "iPhone");

var classics = Product.ContainsInDescription ("Nokia", "Ericsson")
                      .And (Product.IsSelling());
var query =
  from p in Data.Products.Where (newKids.Or (classics))
  select p;
```

The And and Or methods in boldface resolve to extension methods in PredicateBuilder. 
An expression predicate can perform the equivalent of an SQL subquery by referencing association properties. So, if Product had a child EntitySet called Purchases, we could refine our IsSelling method to return only those products that have sold a minimum number of units as follows:

```csharp
public static Expression<Func<Product, bool>> IsSelling (int minPurchases)
{
  return prod =>
    !prod.Discontinued &&
     prod.Purchases.Where (purch => purch.Date > DateTime.Now.AddDays(-30))
                    .Count() >= minPurchases;
}
```

##Nesting Predicates

Consider the following predicate:

```csharp
p => p.Price > 100 &&
     p.Price < 1000 &&
     (p.Description.Contains ("foo") || p.Description.Contains ("far"))
```

Let's say we wanted to build this dynamically. The question is, how do we deal with the parenthesis around the two expressions in the last line?

The answer is to build the parenthesised expression first, and then consume it in the outer expression as follows:

```csharp
var inner = PredicateBuilder.False<Product>();
inner = inner.Or (p => p.Description.Contains ("foo"));
inner = inner.Or (p => p.Description.Contains ("far"));

var outer = PredicateBuilder.True<Product>();
outer = outer.And (p => p.Price > 100);
outer = outer.And (p => p.Price < 1000);
outer = outer.And (inner);
```

Notice that with the inner expression, we start with PredicateBuilder.False (because we're using the Or operator). With the outer expression, however, we start with PredicateBuilder.True (because we're using the And operator).

##Generic Predicates

Suppose every table in your database has ValidFrom and ValidTo columns as follows:

```
create table PriceList
(
   ID int not null primary key,
   Name nvarchar(50) not null,
   ValidFrom datetime,
   ValidTo datetime
)
```

To retrieve rows valid as of DateTime.Now (the most common case), you'd do this:

```
from p in PriceLists
where (p.ValidFrom == null || p.ValidFrom <= DateTime.Now) &&
      (p.ValidTo   == null || p.ValidTo   >= DateTime.Now)
select p.Name
```

Of course, that logic in bold is likely to be duplicated across multiple queries! No problem: let's define a method in the PriceList class that returns a reusable expression:

```csharp
public static Expression<Func<PriceList, bool>> IsCurrent()
{
   return p => (p.ValidFrom == null || p.ValidFrom <= DateTime.Now) &&
               (p.ValidTo   == null || p.ValidTo   >= DateTime.Now);
}
```

OK: our query is now much simpler:

```csharp
var currentPriceLists = db.PriceLists.Where (PriceList.IsCurrent());
```

And with PredicateBuilder's And and Or methods, we can easily introduce other conditions:

```csharp
var currentPriceLists = db.PriceLists.Where (
                          PriceList.IsCurrent().And (p => p.Name.StartsWith ("A")));
```

But what about all the other tables that also have ValidFrom and ValidTo columns? We don't want to repeat our IsCurrent method for every table! Fortunately, we can generalize our IsCurrent method with generics.

The first step is to define an interface:

```csharp
public interface IValidFromTo
{
   DateTime? ValidFrom { get; }
   DateTime? ValidTo   { get; }
}
```

Now we can define a single generic IsCurrent method using that interface as a constraint:

```csharp
public static Expression<Func<TEntity, bool>> IsCurrent<TEntity>()
   where TEntity : IValidFromTo
{
   return e => (e.ValidFrom == null || e.ValidFrom <= DateTime.Now) &&
               (e.ValidTo   == null || e.ValidTo   >= DateTime.Now);
}
```

The final step is to implement this interface in each class that supports ValidFrom and ValidTo. If you're using Visual Studio or a tool like SqlMetal to generate your entity classes, do this in the non-generated half of the partial classes:

```csharp
public partial class PriceList : IValidFromTo { }
public partial class Product   : IValidFromTo { }
```

Original source and author
=======

http://www.albahari.com/nutshell/linqkit.aspx

Permission has been granted to have this repo be the official source for this project.

License
=======
LINQKit is free. The source code is issued under a permissive free license, which means you can modify it as you please, and incorporate it into your own commercial or non-commercial software.
