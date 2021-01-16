using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqKit.Utilities;

namespace LinqKit
{
    /// <summary>
    /// Custom expression visitor for ExpandableQuery. This expands calls to Expression.Compile() and
    /// collapses captured lambda references in subqueries which LINQ to SQL can't otherwise handle.
    /// </summary>
    class ExpressionExpander : ExpressionVisitor
    {

        readonly Dictionary<MemberInfo, LambdaExpression> _expandableCache = new Dictionary<MemberInfo, LambdaExpression>();

        internal ExpressionExpander() { }

        protected LambdaExpression EvaluateTarget(Expression target)
        {
            if (target is LambdaExpression lambdaExpression)
            {
                return lambdaExpression;
            }

            if (target.NodeType == ExpressionType.Call)
            {
                var mc = (MethodCallExpression)target;
                if (mc.Method.Name == "Compile" && mc.Method.DeclaringType?.GetGenericTypeDefinition() == typeof(Expression<>))
                {
                    target = mc.Object;
                }
            }

            var lambda = target.EvaluateExpression() as LambdaExpression;

            if (lambda == null)
            {
                throw new InvalidOperationException($"Invoke cannot evaluate LambdaExpression from '{target}'. Ensure that your function/property/member returns LambdaExpression");
            }

            return lambda;
        }

        /// <summary>
        /// Flatten calls to Invoke so that Entity Framework can understand it. Calls to Invoke are generated
        /// by PredicateBuilder.
        /// </summary>
        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            var target = iv.Expression;

            var lambda = EvaluateTarget(target);

            var body = ExpressionReplacer.GetBody(lambda, iv.Arguments);

            return Visit(body);
        }

        protected bool GetExpandLambda(MemberInfo memberInfo, out LambdaExpression expandLambda)
        {
            if (_expandableCache.TryGetValue(memberInfo, out expandLambda))
            {
                return expandLambda != null;
            }

            var canExpand = memberInfo.DeclaringType != null;
            if (canExpand)
            {
                // shortcut for standard methods
                canExpand = memberInfo.DeclaringType != typeof(Enumerable) &&
                            memberInfo.DeclaringType != typeof(Queryable);
            }

            if (canExpand)
            {
                var attr = memberInfo.GetCustomAttributes(typeof(ExpandableAttribute), true).FirstOrDefault() as ExpandableAttribute;

                if (attr != null)
                {
                    var methodName = string.IsNullOrEmpty(attr.MethodName) ? memberInfo.Name : attr.MethodName;

                    Expression expr;

                    if (memberInfo is MethodInfo method && method.IsGenericMethod)
                    {
                        var args = method.GetGenericArguments();

                        expr = Expression.Call(memberInfo.DeclaringType, methodName, args);
                    }
                    else
                    {
                        expr = Expression.Call(memberInfo.DeclaringType, methodName, new Type[0]);
                    }

                    expandLambda = expr.EvaluateExpression() as LambdaExpression;
                    if (expandLambda == null)
                    {
                        throw new InvalidOperationException(
                            $"Expandable method from '{memberInfo.DeclaringType}.{methodName}()' have returned not a LambdaExpression.");
                    }

                    _expandableCache.Add(memberInfo, expandLambda);
                    return true;
                }
            }

            _expandableCache.Add(memberInfo, null);
            return false;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.Name == "Invoke" && m.Method.DeclaringType == typeof(Extensions))
            {
                var target = m.Arguments[0];
                var lambda = EvaluateTarget(target);

                var replaceVars = new Dictionary<Expression, Expression>();
                for (int i = 0; i < lambda.Parameters.Count; i++)
                {
                    replaceVars.Add(lambda.Parameters[i], Visit(m.Arguments[i + 1]));
                }

                var body = ExpressionReplacer.Replace(lambda.Body, replaceVars);

                return Visit(body);
            }

            if (GetExpandLambda(m.Method, out var methodLambda))
            {
                var replaceVars = new Dictionary<Expression, Expression>();

                if (m.Method.IsStatic)
                {
                    for (int i = 0; i < methodLambda.Parameters.Count; i++)
                    {
                        replaceVars.Add(methodLambda.Parameters[i], m.Arguments[i]);
                    }
                }
                else
                {
                    replaceVars.Add(methodLambda.Parameters[0], m.Object);
                    for (int i = 1; i < methodLambda.Parameters.Count; i++)
                    {
                        replaceVars.Add(methodLambda.Parameters[i], m.Arguments[i - 1]);
                    }
                }

                var newExpr = ExpressionReplacer.Replace(methodLambda.Body, replaceVars);

                return Visit(newExpr);
            }

            // Expand calls to an expression's Compile() method:
            if (m.Method.Name == "Compile" && m.Object is MemberExpression)
            {
                var me = (MemberExpression)m.Object;
                var newExpr = TransformExpr(me);
                if (newExpr != me)
                {
                    return Visit(newExpr);
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
            if (GetExpandLambda(m.Member, out var methodLambda))
            {
                var newExpr = ExpressionReplacer.GetBody(methodLambda, m.Expression);

                return Visit(newExpr);
            }

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