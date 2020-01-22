using System.Linq;
using ConsoleAppNetCore3Ef3.EntityFrameworkCore;
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

            var c = new MyHotelDbContext(optionsBuilder.Options);
            c.Database.EnsureCreated();

            var q = c.Guests.AsExpandable();

            var result = q.ToArray();
            int y = 0;
        }
    }
}
