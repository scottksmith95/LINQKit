#if NOEF
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace LinqKit
{
    /// <summary>
    /// This comes from Matt Warren's sample:
    /// http://blogs.msdn.com/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx
    /// </summary>
    public abstract class ExpressionVisitor
    {
        /// <summary> Visit expression tree </summary>
        [Pure]
		public virtual Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }

#if NET35            
            switch (exp.NodeType)
            {
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    return VisitUnary((UnaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return VisitListInit((ListInitExpression)exp);
#else
            if (exp.NodeType == ExpressionType.Extension)
            {
                return VisitExtension(exp);
            }
            
            switch (exp)
            {
                case UnaryExpression unaryExpression:
                    return VisitUnary(unaryExpression);
                case BinaryExpression binaryExpression:
                    return VisitBinary(binaryExpression);
                case BlockExpression blockExpression:
                    return VisitBlock(blockExpression);
                case TypeBinaryExpression typeBinaryExpression:
                    return VisitTypeIs(typeBinaryExpression);
                case ConditionalExpression conditionalExpression:
                    return VisitConditional(conditionalExpression);
                case ConstantExpression constantExpression:
                    return VisitConstant(constantExpression);
                case DebugInfoExpression debugInfoExpression:
                    return VisitDebugInfo(debugInfoExpression);
                case DefaultExpression defaultExpression:
                    return VisitDefault(defaultExpression);
#if !NETSTANDARD1_3
                case DynamicExpression dynamicExpression:
                    return VisitDynamic(dynamicExpression);
#endif
                case GotoExpression gotoExpression:
                    return VisitGoto(gotoExpression);
                case ParameterExpression parameterExpression:
                    return VisitParameter(parameterExpression);
                case RuntimeVariablesExpression runtimeVariablesExpression:
                    return VisitRuntimeVariables(runtimeVariablesExpression);
                case SwitchExpression switchExpression:
                    return VisitSwitch(switchExpression);
                case TryExpression tryExpression:
                    return VisitTry(tryExpression);
                case MemberExpression memberExpression:
                    return VisitMemberAccess(memberExpression);
                case MethodCallExpression methodCallExpression:
                    return VisitMethodCall(methodCallExpression);
                case LambdaExpression lambdaExpression:
                    return VisitLambda(lambdaExpression);
                case NewExpression newExpression:
                    return VisitNew(newExpression);
                case NewArrayExpression newArrayExpression:
                    return VisitNewArray(newArrayExpression);
                case InvocationExpression invocationExpression:
                    return VisitInvocation(invocationExpression);
                case LabelExpression labelExpression:
                    return VisitLabel(labelExpression);
                case MemberInitExpression memberInitExpression:
                    return VisitMemberInit(memberInitExpression);
                case ListInitExpression listInitExpression:
                    return VisitListInit(listInitExpression);
                case LoopExpression loopExpression:
                    return VisitLoop(loopExpression);
                case IndexExpression indexExpression:
                    return VisitIndex(indexExpression);
#endif
                default:
                    throw new Exception($"Unhandled expression type: '{exp.GetType().Name}': '{exp.NodeType}'");
            }
        }

        /// <summary>
        /// Visit Extension expression to fix bugs:
        /// - https://github.com/scottksmith95/LINQKit/issues/116
        /// - https://github.com/scottksmith95/LINQKit/issues/118
        /// 
        /// TODO (2020-07-16) I'm not sure if just returning the expression will work in all cases...
        /// 
        /// See also https://nejcskofic.github.io/2017/07/30/extending-linq-expressions/
        /// </summary>
        protected virtual Expression VisitExtension(Expression extensionExpression)
        {
            return extensionExpression;
        }

        /// <summary> Visit member binding </summary>
		protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new Exception($"Unhandled binding type '{binding.BindingType}'");
            }
        }

        /// <summary> Visit element initializer </summary>
		protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            ReadOnlyCollection<Expression> arguments = VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments)
            {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }
            return initializer;
        }

        /// <summary> Visit one-parameter expression </summary>
        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            Expression operand = Visit(u.Operand);
            if (operand != u.Operand)
            {
                return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
            }
            return u;
        }

        /// <summary> Visit two-parameter expression </summary>
		protected virtual Expression VisitBinary(BinaryExpression b)
        {
            Expression left = Visit(b.Left);
            Expression right = Visit(b.Right);
            Expression conversion = Visit(b.Conversion);
            if (left != b.Left || right != b.Right || conversion != b.Conversion)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                {
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                }

                return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
            }
            return b;
        }

        /// <summary> Visit type-is expression </summary>
        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            Expression expr = Visit(b.Expression);
            if (expr != b.Expression)
            {
                return Expression.TypeIs(expr, b.TypeOperand);
            }
            return b;
        }

        /// <summary> Return constant expression </summary>
		protected virtual Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

        /// <summary> Simplify conditional expression </summary>
		protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            Expression test = Visit(c.Test);
            var checkbool = test as ConstantExpression;
            if (checkbool?.Value is bool)
            {
                if ((bool)checkbool.Value)
                {
                    return Visit(c.IfTrue);
                }

                return Visit(c.IfFalse);
            }
            Expression ifTrue = Visit(c.IfTrue);
            Expression ifFalse = Visit(c.IfFalse);
            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
            {
                return Expression.Condition(test, ifTrue, ifFalse);
            }
            return c;
        }

        /// <summary> Return parameter expression </summary>
        protected virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        /// <summary> Visit member access </summary>
        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            Expression exp = Visit(m.Expression);
            if (exp != m.Expression)
            {
                return Expression.MakeMemberAccess(exp, m.Member);
            }
            return m;
        }

        /// <summary> Visit method call </summary>
		protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            Expression obj = Visit(m.Object);
            IEnumerable<Expression> args = VisitExpressionList(m.Arguments);
            if (obj != m.Object || args != m.Arguments)
            {
                return Expression.Call(obj, m.Method, args);
            }
            return m;
        }

        /// <summary> Visit list of expressions </summary>
		protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                Expression p = Visit(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }

            if (list != null)
            {
#if (PORTABLE || PORTABLE40)
                return new ReadOnlyCollection<Expression>(list);
#else
                return list.AsReadOnly();
#endif
            }

            return original;
        }

        /// <summary> Visit member assignment </summary>
        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression e = Visit(assignment.Expression);
            if (e != assignment.Expression)
            {
                return Expression.Bind(assignment.Member, e);
            }
            return assignment;
        }

        /// <summary> Visit member binding </summary>
		protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = VisitBindingList(binding.Bindings);
            if (bindings != binding.Bindings)
            {
                return Expression.MemberBind(binding.Member, bindings);
            }
            return binding;
        }

        /// <summary> Visit member list binding </summary>
        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = VisitElementInitializerList(binding.Initializers);
            if (initializers != binding.Initializers)
            {
                return Expression.ListBind(binding.Member, initializers);
            }
            return binding;
        }

        /// <summary> Visit list of bindings </summary>
		protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberBinding b = VisitBinding(original[i]);
                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(b);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        /// <summary> Visit list of element-initializers </summary>
        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                ElementInit init = VisitElementInitializer(original[i]);
                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(init);
                }
            }
            return list != null ? (IEnumerable<ElementInit>)list : original;
        }

        /// <summary> Visit lambda expression </summary>
        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Expression body = Visit(lambda.Body);
            return body != lambda.Body ? Expression.Lambda(lambda.Type, body, lambda.Parameters) : lambda;
        }

        /// <summary> Visit new-expression </summary>
		protected virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> args = VisitExpressionList(nex.Arguments);
            return args != nex.Arguments
                       ? (nex.Members != null
                              ? Expression.New(nex.Constructor, args, nex.Members)
                              : Expression.New(nex.Constructor, args))
                       : nex;
        }

        /// <summary> Visit member init expression </summary>
	    protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression n = VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = VisitBindingList(init.Bindings);
            return n != init.NewExpression || bindings != init.Bindings ? Expression.MemberInit(n, bindings) : init;
        }

        /// <summary> Visit list init expression </summary>
		protected virtual Expression VisitListInit(ListInitExpression init)
        {
            NewExpression n = VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = VisitElementInitializerList(init.Initializers);
            return n != init.NewExpression || initializers != init.Initializers ? Expression.ListInit(n, initializers) : init;
        }

        /// <summary> Visit array expression </summary>
        protected virtual Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> exprs = VisitExpressionList(na.Expressions);
            return exprs != na.Expressions
                       ? (na.NodeType == ExpressionType.NewArrayInit
                              ? Expression.NewArrayInit(na.Type.GetElementType(), exprs)
                              : Expression.NewArrayBounds(na.Type.GetElementType(), exprs))
                       : na;
        }

        /// <summary> Visit invocation expression </summary>
	    protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> args = VisitExpressionList(iv.Arguments);
            Expression expr = Visit(iv.Expression);
            return args != iv.Arguments || expr != iv.Expression ? Expression.Invoke(expr, args) : iv;
        }

#if !NET35
        /// <summary> Visit index expression </summary>
        protected virtual Expression VisitIndex(IndexExpression exp)
        {
            var obj = Visit(exp.Object);
            var args = VisitExpressionList(exp.Arguments);
            if (obj != exp.Object || args != exp.Arguments)
            {
                return Expression.MakeIndex(obj, exp.Indexer, args);
            }
            return exp;
        }

        /// <summary>Visit Block expression</summary>
        protected virtual Expression VisitBlock(BlockExpression exp)
        {
            var expressions = VisitExpressionList(exp.Expressions);
            if (expressions != exp.Expressions)
            {
                return Expression.Block(exp.Type, exp.Variables, expressions);
            }
            return exp;
        }

        /// <summary>Visit DebugInfo expression</summary>
        protected virtual Expression VisitDebugInfo(DebugInfoExpression exp)
        {
            return exp;
        }

        /// <summary>Visit Default expression</summary>
        protected virtual Expression VisitDefault(DefaultExpression exp)
        {
            return exp;
        }

#if !NETSTANDARD1_3
        /// <summary>Visit Dynamic expression</summary>
        protected virtual Expression VisitDynamic(DynamicExpression exp)
        {
            var arguments = VisitExpressionList(exp.Arguments);
            if (arguments != exp.Arguments)
            {
                return Expression.Dynamic(exp.Binder, exp.Type, arguments);
            }
            return exp;
        }
#endif

        /// <summary>Visit Goto expression</summary>
        protected virtual Expression VisitGoto(GotoExpression exp)
        {
            var value = Visit(exp.Value);
            if (value != exp.Value)
            {
                return Expression.MakeGoto(exp.Kind, exp.Target, value, exp.Type);
            }
            return exp;
        }

        /// <summary>Visit RuntimeVariables expression</summary>
        protected virtual Expression VisitRuntimeVariables(RuntimeVariablesExpression exp)
        {
            return exp;
        }

        /// <summary>Visit Switch expression</summary>
        protected virtual Expression VisitSwitch(SwitchExpression exp)
        {
            var switchValue = Visit(exp.SwitchValue);
            var defaultBody = Visit(exp.DefaultBody);
            var cases = VisitSwitchCaseList(exp.Cases);
            if (switchValue != exp.SwitchValue || defaultBody != exp.DefaultBody || cases != exp.Cases)
            {
                return Expression.Switch(exp.Type, switchValue, defaultBody, exp.Comparison, cases);
            }
            return exp;
        }

        /// <summary> Visit list of switch-cases </summary>
        protected virtual ReadOnlyCollection<SwitchCase> VisitSwitchCaseList(ReadOnlyCollection<SwitchCase> original)
        {
            List<SwitchCase> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                SwitchCase p = VisitSwitchCase(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<SwitchCase>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }

            if (list != null)
            {
#if (PORTABLE || PORTABLE40)
                return new ReadOnlyCollection<SwitchCase>(list);
#else
                return list.AsReadOnly();
#endif
            }

            return original;
        }

        /// <summary>Visit SwitchCase</summary>
        protected virtual SwitchCase VisitSwitchCase(SwitchCase exp)
        {
            var body = Visit(exp.Body);
            var testValues = VisitExpressionList(exp.TestValues);
            if (body != exp.Body || testValues != exp.TestValues)
            {
                return Expression.SwitchCase(body, testValues);
            }
            return exp;
        }

        /// <summary>Visit Try expression</summary>
        protected virtual Expression VisitTry(TryExpression exp)
        {
            var body = Visit(exp.Body);
            var fault = Visit(exp.Fault);
            var @finally = Visit(exp.Finally);
            var handlers = VisitCatchBlockList(exp.Handlers);
            if (body != exp.Body || fault != exp.Fault || @finally != exp.Finally || handlers != exp.Handlers)
            {
                return Expression.MakeTry(exp.Type, body, @finally, fault, handlers);
            }
            return exp;
        }

        /// <summary> Visit list of catch-blocks </summary>
        protected virtual ReadOnlyCollection<CatchBlock> VisitCatchBlockList(ReadOnlyCollection<CatchBlock> original)
        {
            List<CatchBlock> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                CatchBlock p = VisitCatchBlock(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<CatchBlock>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }

            if (list != null)
            {
#if (PORTABLE || PORTABLE40)
                return new ReadOnlyCollection<CatchBlock>(list);
#else
                return list.AsReadOnly();
#endif
            }

            return original;
        }

        /// <summary>Visit catch-block</summary>
        protected virtual CatchBlock VisitCatchBlock(CatchBlock exp)
        {
            var body = Visit(exp.Body);
            var filter = Visit(exp.Filter);
            if (body != exp.Body || filter != exp.Filter)
            {
                return Expression.MakeCatchBlock(exp.Test, exp.Variable, body, filter);
            }
            return exp;
        }

        /// <summary>Visit Label expression</summary>
        protected virtual Expression VisitLabel(LabelExpression exp)
        {
            var defaultValue = Visit(exp.DefaultValue);
            if (defaultValue != exp.DefaultValue)
            {
                return Expression.Label(exp.Target, defaultValue);
            }
            return exp;
        }

        /// <summary>Visit Loop expression</summary>
        protected virtual Expression VisitLoop(LoopExpression exp)
        {
            var body = Visit(exp.Body);
            if (body != exp.Body)
            {
                return Expression.Loop(body, exp.BreakLabel, exp.ContinueLabel);
            }
            return exp;
        }
#endif
    }
}
#endif