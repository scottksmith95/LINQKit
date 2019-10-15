using System.Data.Entity;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace LinqKitControversy.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var ef6Ctx = new Ef6Context();
            var foo = await ef6Ctx.FooSet.AsExpandableEF().ToListAsync();

            var coreCtx = new CoreContext();
            var bar = await coreCtx.BarSet.AsExpandableEFCore().ToListAsync();
        }
    }
}
