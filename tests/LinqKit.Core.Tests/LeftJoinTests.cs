using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using LinqKit;
using LinqKitCoreTests.Model;
using Xunit;

// Define specific namespace
// Just to show issue https://github.com/scottksmith95/LINQKit/issues/218
namespace LinqKitCoreTests;

public class LeftJoinTests
{
    [Fact]
    public void LeftJoin()
    {
        var testEntities = new List<TestEntity>
        {
            new() { Id = 1, Value = "One" },
            new() { Id = 2, ParentId = 1, Value = "Two" }
        };

        var outer = testEntities.AsQueryable();
        var inner = testEntities;

        outer.LeftJoin(
            inner,
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

    [Fact]
    public void Expand()
    {
        var exp = Expression.Add(Expression.Constant(5), Expression.Constant(2));
        var executed = exp.Expand().ToString();
        Assert.Equal(exp.ToString(), executed);
    }
}