using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace LinqKit
{
    class ExpressionReplacer : ExpressionVisitor
    {
        private readonly IDictionary<Expression, Expression> _replaceMap;

        public ExpressionReplacer([NotNull] IDictionary<Expression, Expression> replaceMap)
        {
            _replaceMap = replaceMap ?? throw new ArgumentNullException(nameof(replaceMap));
        }

        public override Expression Visit(Expression exp)
        {
            if (exp != null && _replaceMap.TryGetValue(exp, out var replacement))
            {
                return replacement;
            }

            return base.Visit(exp);
        }

        public static Expression Replace(Expression expr, Expression fromExpr, Expression toExpr)
        {
            return new ExpressionReplacer(new Dictionary<Expression, Expression> { { fromExpr, toExpr } }).Visit(expr);
        }

        public static Expression Replace(Expression expr, [NotNull] IDictionary<Expression, Expression> replaceMap)
        {
            return new ExpressionReplacer(replaceMap).Visit(expr);
        }

        public static Expression GetBody(LambdaExpression lambda, params Expression[] toExpressions)
        {
            if (lambda.Parameters.Count != toExpressions.Length)
            {
                throw new InvalidOperationException("Wrong parameter replacement count.");
            }

            var dictionary = new Dictionary<Expression, Expression>();
            for (int i = 0; i < lambda.Parameters.Count; i++)
            {
                dictionary.Add(lambda.Parameters[i], toExpressions[i]);
            }

            return Replace(lambda.Body, dictionary);
        }

        public static Expression GetBody(LambdaExpression lambda, ReadOnlyCollection<Expression> toExpressions)
        {
            if (lambda.Parameters.Count != toExpressions.Count)
            {
                throw new InvalidOperationException("Wrong parameter replacement count.");
            }

            var dictionary = new Dictionary<Expression, Expression>();
            for (int i = 0; i < lambda.Parameters.Count; i++)
            {
                dictionary.Add(lambda.Parameters[i], toExpressions[i]);
            }

            return Replace(lambda.Body, dictionary);
        }
    }
}