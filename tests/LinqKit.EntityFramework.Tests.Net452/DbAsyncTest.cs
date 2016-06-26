using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using LinqKit.EntityFramework.Tests;

#if EFCORE
using Microsoft.EntityFrameworkCore;

namespace LinqKit.Microsoft.EntityFrameworkCore.Tests
#else
using System.Data.Entity;

namespace LinqKit.EntityFramework.Tests.Net452
#endif
{
    public class DbAsyncTest : IDisposable
    {
        private TestContext db;

        public DbAsyncTest()
        {
#if EFCORE
            var builder = new DbContextOptionsBuilder();
            builder.UseSqlite($"Filename=LinqKit.{Guid.NewGuid()}.db");

            db = new TestContext(builder.Options);
            db.Database.EnsureCreated();
#else
            db = new TestContext($"data source=(LocalDB)\\MSSQLLocalDB;attachdbfilename=|DataDirectory|\\LinqKit.{Guid.NewGuid()}.mdf;integrated security=True;connect timeout=30;MultipleActiveResultSets=True;App=EntityFramework");
#endif

            db.Entities.RemoveRange(db.Entities.ToList());
            db.Entities.AddRange(new[]
            {
                new Entity { Value = 123 },
                new Entity { Value = 67 },
                new Entity { Value = 3 }
            });
            db.SaveChanges();
        }

        // Use TestCleanup to run code after each test has run
        public void Dispose()
        {
#if EFCORE
            db.Database.EnsureDeleted();
#else
            db.Database.Delete();
#endif
            db.Dispose();
            db = null;
        }

        [Fact]
        public async Task EnumerateShouldWorkAsync()
        {
            var task = db.Entities.AsExpandable().ToListAsync();
            var result = await task;
            var after = task.Status;

            Assert.Equal(TaskStatus.RanToCompletion, after);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task ExecuteShouldWorkAsync()
        {
            int expected = db.Entities.Sum(e => e.Value);
            var task = db.Entities.AsExpandable().SumAsync(e => e.Value);
            var before = task.Status;
            var result = await task;
            var after = task.Status;

            Assert.Equal(TaskStatus.RanToCompletion, after);
            Assert.Equal(expected, result);
            Assert.NotEqual(TaskStatus.RanToCompletion, before);
        }

        [Fact]
        public async Task ExpressionInvokeTest()
        {
            var eParam = Expression.Parameter(typeof(Entity), "e");
            var eProp = Expression.PropertyOrField(eParam, "Value");

            var conditions =
                (from item in new List<int> { 10, 20, 30, 80 }
                 select Expression.LessThan(eProp, Expression.Constant(item))).Aggregate(Expression.OrElse);

            var combined = Expression.Lambda<Func<Entity, bool>>(conditions, eParam);

            var q = from e in db.Entities.AsExpandable()
                    where combined.Invoke(e)
                    select new { e.Value };

            var res = await q.ToListAsync();

            Assert.Equal(2, res.Count);
            Assert.Equal(67, res.First().Value);
        }
    }
}