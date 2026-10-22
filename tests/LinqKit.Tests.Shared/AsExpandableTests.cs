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

            // Verify: the optimizer runs when Where builds the query and again when First executes it
            optimizerMock.Verify(o => o(It.IsAny<Expression>()), Times.Exactly(2));
        }

        [Fact]
        public void AsExpandableEF_Without_Optimizer_Keeps_The_Custom_Optimizer()
        {
            // Assign
            var optimizerMock = new Mock<Func<Expression, Expression>>();
            optimizerMock.Setup(o => o(It.IsAny<Expression>())).Returns((Expression e) => e);

            var expandable = new[] { 1, 2 }.AsQueryable().AsExpandableEF(optimizerMock.Object);

            // Act
            var again = expandable.AsExpandableEF();
            int result = again.Where(i => i > 1).First();

            // Assert
            Assert.Same(expandable, again);
            Assert.Equal(2, result);

            // Verify: the optimizer runs when Where builds the query and again when First executes it
            optimizerMock.Verify(o => o(It.IsAny<Expression>()), Times.Exactly(2));
        }

        Expression<Func<int, int>> FunctionToExpand()
        {
            return i => i * 2;
        }

        Expression<Func<int, int>> FunctionToExpandWithParameter(int multiplier)
        {
            return i => i * multiplier;
        }

        Expression<Func<int, int>> PropertyToExpand
        {
           get { return i => i * 2; }
        }

        [Fact]
        public void AsExpandable_Method_Invoke()
        {
            // Assign
            var query = new[] { 1, 2 }.AsQueryable();

            // Act
            var result = query.AsExpandable()
                .Select(i => FunctionToExpand().Invoke(i)).ToArray();

            // Assert
            Assert.Equal(2, result[0]);
            Assert.Equal(4, result[1]);
        }

        [Fact]
        public void AsExpandable_MethodWithParameter_Invoke()
        {
            // Assign
            var query = new[] { 1, 2 }.AsQueryable();

            // Act
            var result = query.AsExpandable()
                .Select(i => FunctionToExpandWithParameter(3).Invoke(i)).ToArray();

            // Assert
            Assert.Equal(3, result[0]);
            Assert.Equal(6, result[1]);
        }

        [Fact]
        public void AsExpandable_Property_Invoke()
        {
            // Assign
            var query = new[] { 1, 2 }.AsQueryable();

            // Act
            var result = query.AsExpandable()
                .Select(i => PropertyToExpand.Invoke(i)).ToArray();

            // Assert
            Assert.Equal(2, result[0]);
            Assert.Equal(4, result[1]);
        }

        [Fact]
        public void AsExpandable_Method_Compile()
        {
            // Assign
            var query = new[] { 1, 2 }.AsQueryable();

            // Act
            var result = query.AsExpandable()
                .Select(i => FunctionToExpand().Compile()(i)).ToArray();

            // Assert
            Assert.Equal(2, result[0]);
            Assert.Equal(4, result[1]);
        }

        [Fact]
        public void AsExpandable_MethodWithParameter_Compile()
        {
            // Assign
            var query = new[] { 1, 2 }.AsQueryable();

            // Act
            var result = query.AsExpandable()
                .Select(i => FunctionToExpandWithParameter(3).Compile()(i)).ToArray();

            // Assert
            Assert.Equal(3, result[0]);
            Assert.Equal(6, result[1]);
        }

        [Fact]
        public void AsExpandable_Property_Compile()
        {
            // Assign
            var query = new[] { 1, 2 }.AsQueryable();

            // Act
            var result = query.AsExpandable()
                .Select(i => PropertyToExpand.Compile()(i)).ToArray();

            // Assert
            Assert.Equal(2, result[0]);
            Assert.Equal(4, result[1]);
        }

    }
}