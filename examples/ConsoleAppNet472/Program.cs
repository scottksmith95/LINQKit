using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppNetCore3Ef3.EntityFrameworkCore;
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

            var q = c.Guests.AsExpandable();

            var result = q.ToArray();
            int y = 0;
        }
    }
}
