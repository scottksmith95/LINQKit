using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqKit
{
    /// <summary>
    /// Custom expresssion visitor for ExpandableQuery. This expands calls to Expression.Compile() and
    /// collapses captured lambda references in subqueries which LINQ to SQL can't otherwise handle.
    /// </summary>
    class ExpressionExpander : ExpressionVisitor
    {
        // Replacement parameters - for when invoking a lambda expression.
        readonly Dictionary<ParameterExpression, Expression> _replaceVars;

        internal ExpressionExpander() { }

        private ExpressionExpander(Dictionary<ParameterExpression, Expression> replaceVars)
        {
            _replaceVars = replaceVars;
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            return _replaceVars != null && _replaceVars.ContainsKey(p) ? _replaceVars[p] : base.VisitParameter(p);
        }

        /// <summary>
        /// Flatten calls to Invoke so that Entity Framework can understand it. Calls to Invoke are generated
        /// by PredicateBuilder.
        /// </summary>
        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            var target = iv.Expression;
            if (target is MemberExpression) target = TransformExpr((MemberExpression)target);
            if (target is ConstantExpression) target = ((ConstantExpression)target).Value as Expression;

            var lambda = (LambdaExpression)target;

            var replaceVars = _replaceVars == null ?
                new Dictionary<ParameterExpression, Expression>()
                : new Dictionary<ParameterExpression, Expression>(_replaceVars);

            try
            {
                for (int i = 0; i < lambda.Parameters.Count; i++)
                {
                    replaceVars.Add(lambda.Parameters[i], Visit(iv.Arguments[i]));
                }
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException("Invoke cannot be called recursively - try using a temporary variable.", ex);
            }

            return new ExpressionExpander(replaceVars).Visit(lambda.Body);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.Name == "Invoke" && m.Method.DeclaringType == typeof(Extensions))
            {
                var target = m.Arguments[0];
                if (target is MemberExpression)
                {
                    target = TransformExpr((MemberExpression)target);
                }
                if (target is ConstantExpression)
                {
                    target = ((ConstantExpression)target).Value as Expression;
                }
                if (target is UnaryExpression)
                {
                    target = ((UnaryExpression)target).Operand;
                }

                var lambda = (LambdaExpression)target;

                if (lambda != null)
                {
                    var replaceVars = _replaceVars == null ? new Dictionary<ParameterExpression, Expression>() : new Dictionary<ParameterExpression, Expression>(_replaceVars);

                    try
                    {
                        for (int i = 0; i < lambda.Parameters.Count; i++)
                        {
                            replaceVars.Add(lambda.Parameters[i], Visit(m.Arguments[i + 1]));
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        throw new InvalidOperationException("Invoke cannot be called recursively - try using a temporary variable.", ex);
                    }

                    return new ExpressionExpander(replaceVars).Visit(lambda.Body);
                }
            }

            // Expand calls to an expression's Compile() method:
            if (m.Method.Name == "Compile" && m.Object is MemberExpression)
            {
                var me = (MemberExpression)m.Object;
                var newExpr = TransformExpr(me);
                if (newExpr != me)
                {
                    return newExpr;
                }
            }

            // Strip out any nested calls to AsExpandable():
            if (m.Method.Name == "AsExpandable" && m.Method.DeclaringType == typeof(Extensions))
            {
                return m.Arguments[0];
            }

            return base.VisitMethodCall(m);
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            // Strip out any references to expressions captured by outer variables - LINQ to SQL can't handle these:
            return m.Member.DeclaringType != null && m.Member.DeclaringType.Name.StartsWith("<>") ?
                TransformExpr(m)
                : base.VisitMemberAccess(m);
        }

        Expression TransformExpr(MemberExpression input)
        {
            if (input == null)
            {
                return null;
            }

            var field = input.Member as FieldInfo;

            if (field == null)
            {
                if (_replaceVars != null && input.Expression is ParameterExpression && _replaceVars.ContainsKey(input.Expression as ParameterExpression))
                {
                    return base.VisitMemberAccess(input);
                }

                return input;
            }
#if EFCORE || NETSTANDARD || WINDOWS_APP || PORTABLE || UAP
            //Collapse captured outer variables
            if (input.Member.DeclaringType != null && (!input.Member.DeclaringType.GetTypeInfo().IsNestedPrivate
                || !input.Member.DeclaringType.Name.StartsWith("<>"))) // captured outer variable
            {
                return TryVisitExpressionFunc(input, field);
            }
#else
            // Collapse captured outer variables
            if (input.Member.ReflectedType != null && (!input.Member.ReflectedType.IsNestedPrivate
                || !input.Member.ReflectedType.Name.StartsWith("<>"))) // captured outer variable
            {
                return TryVisitExpressionFunc(input, field);
            }
#endif

            var expression = input.Expression as ConstantExpression;
            if (expression != null)
            {
                var obj = expression.Value;
                if (obj == null)
                {
                    return input;
                }

                var t = obj.GetType();
                if (!t.GetTypeInfo().IsNestedPrivate || !t.Name.StartsWith("<>"))
                {
                    return input;
                }

                var fi = (FieldInfo)input.Member;
                var result = fi.GetValue(obj);
                var exp = result as Expression;
                if (exp != null)
                {
                    return Visit(exp);
                }
            }

            return TryVisitExpressionFunc(input, field);
        }

        private Expression TryVisitExpressionFunc(MemberExpression input, FieldInfo field)
        {
            var propertyInfo = input.Member as PropertyInfo;
            if (field.FieldType.GetTypeInfo().IsSubclassOf(typeof(Expression)) || propertyInfo != null && propertyInfo.PropertyType.GetTypeInfo().IsSubclassOf(typeof(Expression)))
            {
                return Visit(Expression.Lambda<Func<Expression>>(input).Compile()());
            }

            return input;
        }
    }
}