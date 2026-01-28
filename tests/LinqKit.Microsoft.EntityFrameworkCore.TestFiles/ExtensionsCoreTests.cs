using FluentAssertions;
using LinqKit.Microsoft.EntityFrameworkCore.Tests.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;
#if NET10_0_OR_GREATER
using System.Linq;
#endif

namespace LinqKit.Microsoft.EntityFrameworkCore.Tests;

public class ExtensionsCoreTests
{
    [Fact]
    public void LeftJoinTest()
    {
        var builder = new DbContextOptionsBuilder<TestContext>();
        builder.UseSqlite("DataSource=testdb;mode=memory");

        using var context = new TestContext(builder.Options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        context.TestEntities.Add(new TestEntity { Id = 1, Value = "One" });
        context.TestEntities.Add(new TestEntity { Id = 2, ParentId = 1, Value = "Two" });
        context.SaveChanges();

        context.TestEntities.LeftJoin(
                context.TestEntities,
                entity => entity.ParentId,
                parentEntity => parentEntity.Id,
                (entity, parentEntity) => new
                {
                    entity.Value,
                    ParentValue = parentEntity == null ? null : parentEntity.Value
                })
            .Should()
            .BeEquivalentTo(
            [
                new {Value = "One", ParentValue = (string) null},
                    new {Value = "Two", ParentValue = "One"}
            ], string.Empty); //message is to select the generic overload
    }
}