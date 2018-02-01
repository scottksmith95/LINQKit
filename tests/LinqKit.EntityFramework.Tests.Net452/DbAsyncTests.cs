using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using Xunit;

#if EFCORE
using Microsoft.EntityFrameworkCore;
using LinqKit.EntityFramework.Tests;

namespace LinqKit.Microsoft.EntityFrameworkCore.Tests
#else
using System.Data.Entity;

namespace LinqKit.EntityFramework.Tests.Net452
#endif
{
    public class DbAsyncTests : IDisposable
    {
        private TestContext _db;

        public DbAsyncTests()
        {
#if EFCORE
            var builder = new DbContextOptionsBuilder();
            builder.UseSqlite($"Filename=LinqKit.{Guid.NewGuid()}.db");

            _db = new TestContext(builder.Options);
            _db.Database.EnsureCreated();
#else
            _db = new TestContext($"data source=(LocalDB)\\MSSQLLocalDB;attachdbfilename=|DataDirectory|\\LinqKit.{Guid.NewGuid()}.mdf;integrated security=True;connect timeout=30;MultipleActiveResultSets=True;App=EntityFramework");
#endif

            _db.Entities.RemoveRange(_db.Entities.ToList());
            _db.Entities.AddRange(new[]
            {
                new Entity { Value = 123 },
                new Entity { Value = 67 },
                new Entity { Value = 3 }
            });
            _db.SaveChanges();
        }

        // Use TestCleanup to run code after each test has run
        public void Dispose()
        {
#if EFCORE
            _db.Database.EnsureDeleted();
#else
            _db.Database.Delete();
#endif
            _db.Dispose();
            _db = null;
        }

        [Fact]
        public async Task DbAsync_EnumerateShouldWorkAsync()
        {
            var task = _db.Entities.AsExpandable().ToListAsync();
            var result = await task.ConfigureAwait(false);
            var after = task.Status;

            Assert.Equal(TaskStatus.RanToCompletion, after);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task DbAsync_ExecuteShouldWorkAsync()
        {
            int expected = _db.Entities.Sum(e => e.Value);
            var task = _db.Entities.AsExpandable().SumAsync(e => e.Value);
            var result = await task.ConfigureAwait(false);
            var after = task.Status;

            Assert.Equal(TaskStatus.RanToCompletion, after);
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task DbAsync_ExpressionInvokeTest()
        {
            var eParam = Expression.Parameter(typeof(Entity), "e");
            var eProp = Expression.PropertyOrField(eParam, "Value");

            var conditions =
                (from item in new List<int> { 10, 20, 30, 80 }
                 select Expression.LessThan(eProp, Expression.Constant(item))).Aggregate(Expression.OrElse);

            var combined = Expression.Lambda<Func<Entity, bool>>(conditions, eParam);

            var q = from e in _db.Entities.AsExpandable()
                    where combined.Invoke(e)
                    select new { e.Value };

            var res = await q.ToListAsync().ConfigureAwait(false);

            Assert.Equal(2, res.Count);
            Assert.Equal(67, res.First().Value);
        }

        [Fact]
        public async Task DbAsync_ExpressionStarter()
        {
            var combined = PredicateBuilder.New<Entity>();
            foreach (int i in new[] { 10, 20, 30, 80 })
            {
                var predicate = PredicateBuilder.New<Entity>(e => e.Value < i);
                combined = combined.Extend(predicate);
            }

            var q = _db.Entities.AsExpandable().Where(combined);
            var res = await q.ToListAsync().ConfigureAwait(false);

            Assert.Equal(2, res.Count);
            Assert.Equal(67, res.First().Value);
        }

        [Fact]
        public async Task DbAsync_ExpressionStarter_With_Optimizer()
        {
            // Assign
            var optimizerMock = new Mock<Func<Expression, Expression>>();
            optimizerMock.Setup(o => o(It.IsAny<Expression>())).Returns((Expression e) => e);

            var combined = PredicateBuilder.New<Entity>();
            foreach (int i in new[] { 10, 20, 30, 80 })
            {
                var predicate = PredicateBuilder.New<Entity>(e => e.Value < i);
                combined = combined.Extend(predicate);
            }

            // Act
            var q = _db.Entities.AsExpandable(optimizerMock.Object).Where(combined);
            var res = await q.ToListAsync().ConfigureAwait(false);

            // Assert
            Assert.Equal(2, res.Count);
            Assert.Equal(67, res.First().Value);

            // Verify
            optimizerMock.Verify(o => o(It.IsAny<Expression>()), Times.Once);
        }
    }
}