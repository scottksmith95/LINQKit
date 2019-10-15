using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LinqKitControversy.Console
{
    public class Bar
    {

    }
    public class CoreContext : DbContext
    {
        public DbSet<Bar> BarSet { get; set; }
    }
}
