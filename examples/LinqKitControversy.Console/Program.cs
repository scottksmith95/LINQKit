using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
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

            Expression<Func<Bar, bool>> expressionEf6 = (b) => b != null;
            var resultEf6 = expressionEf6.InvokeEF(null);

            var coreCtx = new CoreContext();
            var bar = await coreCtx.BarSet.AsExpandableEFCore().ToListAsync();

            Expression<Func<Bar, bool>> expressionCore = (b) => b != null;
            var resultEfCore = expressionCore.InvokeEFCore(null);
        }
    }
}