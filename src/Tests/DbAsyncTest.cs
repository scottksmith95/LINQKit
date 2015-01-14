using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LinqKit.Tests
{
    public class DbAsyncTest
    {
        private readonly TestContext db = new TestContext();

        public DbAsyncTest()
        {
            db.Entities.RemoveRange(db.Entities.ToList());
            db.Entities.AddRange(new[]
            {
                new Entity { Value = 123.45m },
                new Entity { Value = 67.89m },
                new Entity { Value = 3.14m }
            });
            db.SaveChanges();
        }

        [Fact]
        public async Task EnumerateShouldWorkAsync()
        {
            var task = db.Entities.AsExpandable().ToListAsync();
            var before = task.Status;
            var result = await task;
            var after = task.Status;

            Assert.Equal(TaskStatus.RanToCompletion, after);
            Assert.Equal(3, result.Count);
            Assert.NotEqual(TaskStatus.RanToCompletion, before);
        }

        [Fact]
        public async Task ExecuteShouldWorkAsync()
        {
            var task = db.Entities.AsExpandable().SumAsync(e => e.Value);
            var before = task.Status;
            var result = await task;
            var after = task.Status;

            Assert.Equal(TaskStatus.RanToCompletion, after);
            Assert.Equal(194.48m, result, 2);
            Assert.NotEqual(TaskStatus.RanToCompletion, before);
        }

        public class TestContext : DbContext
        {
            public DbSet<Entity> Entities { get; set; }
        }

        public class Entity
        {
            public int Id { get; set; }

            public decimal Value { get; set; }
        }
    }
}
