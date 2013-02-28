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
* AsExpandable is based on a very clever project by Tomas Petricek. ExpressionVisitor comes from a sample by Matt Warren.

LINQKit includes full source code, and is released under a permissive free license.
