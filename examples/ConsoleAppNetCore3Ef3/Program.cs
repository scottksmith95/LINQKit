using System;
using System.Linq;
using System.Linq.Expressions;
using ConsoleAppNetCore3Ef3.EntityFrameworkCore;
using ConsoleAppNetCore3Ef3.EntityFrameworkCore.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace ConsoleAppNetCore3Ef3
{
    class Program
    {
        static void Main(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyHotelDbContext>();
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MyHotel;Trusted_Connection=True;MultipleActiveResultSets=true");

            var context = new MyHotelDbContext(optionsBuilder.Options);
            context.Database.EnsureCreated();

            var predicate = PredicateBuilder.New<Guest>().Or(asset => asset.Name.Contains("e"));
            var predicateQueryable = context.Guests.AsExpandable().Where(predicate);
            Console.WriteLine($"predicateQueryable = '{predicateQueryable}'");

            foreach (var result in predicateQueryable.ToArray())
            {
                Console.WriteLine($"predicateResult: {result.Name}");
            }


            Expression<Func<Guest, bool>> criteria1 = guest => guest.Name.Contains("af");
            Expression<Func<Guest, bool>> criteria2 = guest => criteria1.Invoke(guest) || guest.Id > 1;

            Console.WriteLine($"criteria2 = '{criteria2.Expand()}'");

            var q = context.Guests.AsExpandable().Where(criteria2);

            var results = q.ToArray();
            foreach (var result in results)
            {
                Console.WriteLine($"{result.Name}");
            }
            int y = 0;
        }
    }
}
