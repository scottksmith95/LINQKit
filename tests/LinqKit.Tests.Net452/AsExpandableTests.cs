using System;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using Xunit;

namespace LinqKit.Tests.Net452
{
    public class AsExpandableTests
    {
        [Fact]
        public void AsExpandable_With_Default_Optimizer()
        {
            // Assign
            var query = new[] { 1, 2 }.AsQueryable();

            var first = PredicateBuilder.New<int>(i => i == 1);
            Expression<Func<int, bool>> second = i => i > 0;
            var predicate = first.Extend(second);

            // Act
            int result = query.AsExpandable().Where(predicate).First();

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void AsExpandable_With_Optimizer()
        {
            // Assign
            var optimizerMock = new Mock<Func<Expression, Expression>>();
            optimizerMock.Setup(o => o(It.IsAny<Expression>())).Returns((Expression e) => e);

            var query = new[] { 1, 2 }.AsQueryable();

            var first = PredicateBuilder.New<int>(i => i == 1);
            Expression<Func<int, bool>> second = i => i > 0;
            var predicate = first.Extend(second);

            // Act
            int result = query.AsExpandable(optimizerMock.Object).Where(predicate).First();

            // Assert
            Assert.Equal(1, result);

            // Verify
            optimizerMock.Verify(o => o(It.IsAny<Expression>()), Times.Once);
        }
    }
}