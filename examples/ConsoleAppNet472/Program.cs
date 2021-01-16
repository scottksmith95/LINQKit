using System;
using System.Linq;
using System.Linq.Expressions;
using ConsoleAppNetCore3Ef3.EntityFrameworkCore;
using ConsoleAppNetCore3Ef3.EntityFrameworkCore.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace ConsoleAppNet472
{
    class Program
    {
        static void Main(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyHotelDbContext>();
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MyHotel;Trusted_Connection=True;MultipleActiveResultSets=true");

            var c = new MyHotelDbContext(optionsBuilder.Options);
            c.Database.EnsureCreated();

            Expression<Func<Guest, bool>> criteria1 = guest => guest.Name.Contains("af");
            Expression<Func<Guest, bool>> criteria2 = guest => criteria1.Invoke(guest) || guest.Id > 1;

            Console.WriteLine($"criteria2 = '{criteria2.Expand()}'");

            var q = c.Guests.AsExpandable().Where(criteria2);

            var results = q.ToArray();
            foreach (var result in results)
            {
                Console.WriteLine($"{result.Name}");
            }
            int y = 0;
        }
    }
}