#if !(NET35 || NOASYNCPROVIDER)
using LinqKit.Utilities;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace System.Linq
{
    /// <summary> Extension methods for expression tree balancing. </summary>
    public static class AggregateExtensions
    {
        /// <summary>
        /// Generates balanced binary trees for list of conditions.
        /// E.g.: AndAlso or OrElse
        /// The reason is avoid StackOverFlowExceptions:
        /// var result = lambdas.Aggregate(AndAlso); // StackOverflow when lambdas.Length is 20 000
        /// var result = lambdas.AggregateBalanced(AndAlso); // Ok still when lambdas.Length is 1 000 000
        /// </summary>
        public static TExpression AggregateBalanced<TExpression>(this TExpression[] lambdas, Func<Expression, Expression, TExpression> operationToDo)
            where TExpression : Expression
        {
            var items = lambdas.Length;
            switch (items)
            {
                case 0: throw new InvalidOperationException("Sequence contains no elements");
                case 1: return lambdas[0];
                case 2: return operationToDo(lambdas[0], lambdas[1]);
                default:
                    var half = items / 2;
                    var o1 = AggregateBalanced(lambdas.Take(half).ToArray(), operationToDo);
                    var o2 = AggregateBalanced(lambdas.Skip(half).ToArray(), operationToDo);
                    return operationToDo(o1, o2);
            }
        }

        /// <summary>
        /// Generates balanced binary trees for list of conditions. Generic version.
        /// E.g.: AndAlso or OrElse
        /// The reason is avoid StackOverFlowExceptions:
        /// var result = lambdas.Aggregate(AndAlso); // StackOverflow when lambdas.Length is 20 000
        /// var result = lambdas.AggregateBalanced(AndAlso); // Ok still when lambdas.Length is 1 000 000
        /// </summary>
        public static Expression<T> AggregateBalanced<T>(this Expression<T>[] lambdas, Func<Expression<T>, Expression<T>, Expression<T>> operationToDo)
        {
            var items = lambdas.Length;
            switch (items)
            {
                case 0: throw new InvalidOperationException("Sequence contains no elements");
                case 1: return lambdas[0];
                case 2: return operationToDo(lambdas[0], lambdas[1]);
                default:
                    var half = items / 2;
                    var o1 = AggregateBalanced(lambdas.Take(half).ToArray(), operationToDo);
                    var o2 = AggregateBalanced(lambdas.Skip(half).ToArray(), operationToDo);
                    return operationToDo(o1, o2);
            }
        }

        /// <summary>
        /// Generates balanced binary trees for list of conditions.
        /// E.g.: AndAlso or OrElse
        /// The reason is avoid StackOverFlowExceptions:
        /// var result = lambdas.Aggregate(AndAlso); // StackOverflow when lambdas.Length is 20 000
        /// var result = lambdas.AggregateBalanced(AndAlso); // Ok still when lambdas.Length is 1 000 000
        /// </summary>
        public static async Task<TExpression> AggregateBalancedAsync<TExpression>(this TExpression[] lambdas, Func<Expression, Expression, TExpression> operationToDo)
            where TExpression : Expression
        {
            var items = lambdas.Length;
            switch (items)
            {
                case 0: throw new InvalidOperationException("Sequence contains no elements");
                case 1: return lambdas[0];
                case 2: return operationToDo(lambdas[0], lambdas[1]);
                default:
                    var half = items / 2;
                    var op1 = TaskHelper.Run(() => AggregateBalanced(lambdas.Take(half).ToArray(), operationToDo));
                    var op2 = TaskHelper.Run(() => AggregateBalanced(lambdas.Skip(half).ToArray(), operationToDo));
                    return operationToDo(await op1, await op2);
            }
        }

        /// <summary>
        /// Generates balanced binary trees for list of conditions. Generic version.
        /// E.g.: AndAlso or OrElse
        /// The reason is avoid StackOverFlowExceptions:
        /// var result = lambdas.Aggregate(AndAlso); // StackOverflow when lambdas.Length is 20 000
        /// var result = lambdas.AggregateBalanced(AndAlso); // Ok still when lambdas.Length is 1 000 000
        /// </summary>
        public static async Task<Expression<T>> AggregateBalancedAsync<T>(this Expression<T>[] lambdas, Func<Expression<T>, Expression<T>, Expression<T>> operationToDo)
        {
            var items = lambdas.Length;
            switch (items)
            {
                case 0: throw new InvalidOperationException("Sequence contains no elements");
                case 1: return lambdas[0];
                case 2: return operationToDo(lambdas[0], lambdas[1]);
                default:
                    var half = items / 2;
                    var op1 = TaskHelper.Run(() => AggregateBalanced(lambdas.Take(half).ToArray(), operationToDo));
                    var op2 = TaskHelper.Run(() => AggregateBalanced(lambdas.Skip(half).ToArray(), operationToDo));
                    return operationToDo(await op1, await op2);
            }
        }
    }
}
#endif