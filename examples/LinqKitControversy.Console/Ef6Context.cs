using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqKitControversy.Console
{
    public class Foo
    {

    }
    public class Ef6Context : DbContext
    {
        public IDbSet<Foo> FooSet { get; set; }
    }
}
