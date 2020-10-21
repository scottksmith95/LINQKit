using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqKit.Utilities
{
    internal static class ExpressionHelpers
    {
        public static object EvaluateExpression(this Expression expr)
        {
            if (expr == null)
            {
                return null;
            }

            switch (expr.NodeType)
            {
                case ExpressionType.Constant:
                    return ((ConstantExpression)expr).Value;

                case ExpressionType.MemberAccess:
                {
                    var member = (MemberExpression) expr;

                    if (member.Member is FieldInfo field)
                    {
                        return field.GetValue(member.Expression.EvaluateExpression());
                    }

                    if (member.Member is PropertyInfo property)
                    {
                        return property.GetValue(member.Expression.EvaluateExpression(), null);
                    }

                    break;
                }
                case ExpressionType.Call:
                {
                    var mc = (MethodCallExpression)expr;
                    var arguments = mc.Arguments.Select(EvaluateExpression).ToArray();
                    var instance  = mc.Object.EvaluateExpression();
                    return mc.Method.Invoke(instance, arguments);
                }
            }

            return Expression.Lambda(expr).Compile().DynamicInvoke();
        }
        
    }
}