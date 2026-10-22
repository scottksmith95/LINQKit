using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using Xunit;

namespace LinqKit.Microsoft.EntityFrameworkCore.Tests;

public class AsExpandableEFCoreTests
{
    [Fact]
    public void AsExpandableEFCore_Without_Optimizer_Keeps_The_Custom_Optimizer()
    {
        var optimizerMock = new Mock<Func<Expression, Expression>>();
        optimizerMock.Setup(o => o(It.IsAny<Expression>())).Returns((Expression e) => e);

        var expandable = new[] { 1, 2 }.AsQueryable().AsExpandableEFCore(optimizerMock.Object);

        var again = expandable.AsExpandableEFCore();
        var result = again.Where(i => i > 1).First();

        again.Should().BeSameAs(expandable);
        result.Should().Be(2);

        // The optimizer runs when Where builds the query and again when First executes it
        optimizerMock.Verify(o => o(It.IsAny<Expression>()), Times.Exactly(2));
    }
}
