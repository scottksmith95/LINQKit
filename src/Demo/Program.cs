using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using LinqKit;

namespace Demo
{
	/// <summary>
	/// Demo program for LINQKit.  Refer to http://www.albahari.com/nutshell/linqkit.html for more info.
	/// </summary>
	class Program
	{
		static void Main ()
		{
			Console.WriteLine ("Customers who have made purchases over $1000");
			QueryCustomers (p => p.Price > 1000);

			Console.WriteLine ();
			Console.WriteLine ("Customers who have made purchases over $1000 (manual join)");
			QueryCustomersManualJoin (p => p.Price > 1000);

			Console.WriteLine ();
			Console.WriteLine ("Customers who have made purchases over $1000 (manual join 2)");
			QueryCustomersManualJoin2 (p => p.Price > 1000);

			Console.WriteLine ();
			TestExpressionCombiner ();

			Console.WriteLine ();
			Console.WriteLine ("Done");
			Console.ReadLine ();
		}

		/// <summary>
		/// This demonstrates how to plug a query expression into an EntitySet. It prints all customers and the
		/// value of their purchases, filtering the purchases according to a specified predicate.
		/// </summary>
		static void QueryCustomers (Expression<Func<Purchase, bool>> purchasePredicate)
		{
			var data = new DemoData ();

			// We do two special things to make this query work:
			//   (1) Call AsExpandable() on the Customer table
			//   (2) Call Compile() on the supplied predicate when it's used within an EntitySet.
			// AsExpandable() returns a wrapper that strips away the call to Compile when the query is run.

			var query =
				from c in data.Customers.AsExpandable ()
				where c.Purchases.Any (purchasePredicate.Compile ())
				select new
				{
					c.Name,
					FilteredPurchases =
						from p in c.Purchases.Where (purchasePredicate.Compile ())
						select p.Price
				};

			foreach (var customerResult in query)
			{
				Console.WriteLine (customerResult.Name);
				foreach (decimal price in customerResult.FilteredPurchases)
					Console.WriteLine ("   $" + price);
			}
		}

		/// <summary>
		/// This gives the same result as the proceeding example, except that instead of using
		/// the customer's Purchases association property, we do a manual join.  Our AsExpandable 
		/// wrapper, here, serves a different role: it lets us reference purchasePredicate within a
		/// subquery (LINQ to SQL ordinarily disallows this). 
		/// </summary>
		static void QueryCustomersManualJoin (Expression<Func<Purchase, bool>> purchasePredicate)
		{
			var data = new DemoData ();

			// We only need to call AsExpandable() on the Customer table to make this query work. Try
			// running this without AsExpandable() and watch LINQ to SQL squirm!

			var query =
				from c in data.Customers.AsExpandable ()
				let custPurchases = data.Purchases.Where (p => p.CustomerID == c.ID)		// manual join
				where custPurchases.Any (purchasePredicate)
				select new
				{
					c.Name,
					FilteredPurchases =
						from p in custPurchases.Where (purchasePredicate)
						select p.Price
				};

			foreach (var customerResult in query)
			{
				Console.WriteLine (customerResult.Name);
				foreach (decimal price in customerResult.FilteredPurchases)
					Console.WriteLine ("   $" + price);
			}
		}

		/// <summary>
		/// Another way to formulate the preceding query.
		/// </summary>
		static void QueryCustomersManualJoin2 (Expression<Func<Purchase, bool>> purchasePredicate)
		{
			var data = new DemoData ();

			// Here we're including the purchasePredicate in the filteredPurchases expression. 
			// We can combine lambda expressions by calling Invoke() on the variable
			// expresssion.  The AsExpandable() wrapper then strips out the call to Invoke
			// and emits one clean expression:

			var query =
				from c in data.Customers.AsExpandable ()
				let filteredPurchases = data.Purchases.Where (
					p => p.CustomerID == c.ID && purchasePredicate.Invoke (p))
				where filteredPurchases.Any ()
				select new
				{
					c.Name,
					FilteredPurchases =
						from p in filteredPurchases
						select p.Price
				};

			foreach (var customerResult in query)
			{
				Console.WriteLine (customerResult.Name);
				foreach (decimal price in customerResult.FilteredPurchases)
					Console.WriteLine ("   $" + price);
			}
		}

		/// <summary>
		/// A simple example of how to combine expressions (i.e., how to have one expression call another).
		/// </summary>
		static void TestExpressionCombiner ()
		{
			var data = new DemoData ();

			Expression<Func<Purchase, bool>> criteria1 = p => p.Price > 1000;
			Expression<Func<Purchase, bool>> criteria2 = p => criteria1.Invoke (p) || p.Description.Contains ("a");

			Console.WriteLine ("Here's what criteria2 looks like, before calling Expand");
			Console.WriteLine (criteria2.ToString ());

			Console.WriteLine ();
			Console.WriteLine ("Here's what criteria2 looks like, after calling Expand");
			Console.WriteLine (criteria2.Expand ().ToString ());

			// We can use criteria2 in either of two ways: either call Expand on the expression before using it:
			var query = data.Purchases.Where (criteria2.Expand ());
			Console.WriteLine ("Count: " + query.Count ());

			// or call AsExpandable() on the Table:
			query = data.Purchases.AsExpandable ().Where (criteria2);
			Console.WriteLine ("Count: " + query.Count ());
		}
	}
}
