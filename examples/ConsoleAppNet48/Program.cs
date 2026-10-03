using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;

namespace ConsoleAppNet48
{
    public class Guest
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class HotelContext : DbContext
    {
        public HotelContext() : base(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LinqKitConsoleAppNet48;Integrated Security=True")
        {
        }

        public DbSet<Guest> Guests { get; set; }
    }

    class Program
    {
        static void Main()
        {
            using (var context = new HotelContext())
            {
                if (context.Database.CreateIfNotExists())
                {
                    context.Guests.Add(new Guest { Name = "Stefan" });
                    context.Guests.Add(new Guest { Name = "Rafael" });
                    context.Guests.Add(new Guest { Name = "Scott" });
                    context.SaveChanges();
                }

                Expression<Func<Guest, bool>> criteria1 = guest => guest.Name.Contains("af");
                Expression<Func<Guest, bool>> criteria2 = guest => criteria1.Invoke(guest) || guest.Id > 2;

                Console.WriteLine($"criteria2 = '{criteria2.Expand()}'");

                var predicate = PredicateBuilder.New<Guest>(false).Or(criteria2);

                foreach (var guest in context.Guests.AsExpandable().Where(predicate))
                {
                    Console.WriteLine(guest.Name);
                }
            }
        }
    }
}
