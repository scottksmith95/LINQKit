using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LinqKit
{
	/// <summary>
	/// This comes from Matt Warren's sample:
	/// http://blogs.msdn.com/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx
	/// </summary>
	public abstract class ExpressionVisitor
	{
        /// <summary> Visit expression tree </summary>
		public virtual Expression Visit (Expression exp)
		{
			if (exp == null)
				return null;

			switch (exp.NodeType)
			{
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
				case ExpressionType.Not:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.ArrayLength:
				case ExpressionType.Quote:
				case ExpressionType.TypeAs:
					return this.VisitUnary ((UnaryExpression)exp);
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
				case ExpressionType.Divide:
				case ExpressionType.Modulo:
				case ExpressionType.And:
				case ExpressionType.AndAlso:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.Equal:
				case ExpressionType.NotEqual:
				case ExpressionType.Coalesce:
				case ExpressionType.ArrayIndex:
				case ExpressionType.RightShift:
				case ExpressionType.LeftShift:
				case ExpressionType.ExclusiveOr:
					return this.VisitBinary ((BinaryExpression)exp);
				case ExpressionType.TypeIs:
					return this.VisitTypeIs ((TypeBinaryExpression)exp);
				case ExpressionType.Conditional:
					return this.VisitConditional ((ConditionalExpression)exp);
				case ExpressionType.Constant:
					return this.VisitConstant ((ConstantExpression)exp);
				case ExpressionType.Parameter:
					return this.VisitParameter ((ParameterExpression)exp);
				case ExpressionType.MemberAccess:
					return this.VisitMemberAccess ((MemberExpression)exp);
				case ExpressionType.Call:
					return this.VisitMethodCall ((MethodCallExpression)exp);
				case ExpressionType.Lambda:
					return this.VisitLambda ((LambdaExpression)exp);
				case ExpressionType.New:
					return this.VisitNew ((NewExpression)exp);
				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
					return this.VisitNewArray ((NewArrayExpression)exp);
				case ExpressionType.Invoke:
					return this.VisitInvocation ((InvocationExpression)exp);
				case ExpressionType.MemberInit:
					return this.VisitMemberInit ((MemberInitExpression)exp);
				case ExpressionType.ListInit:
					return this.VisitListInit ((ListInitExpression)exp);
				default:
					throw new Exception (string.Format ("Unhandled expression type: '{0}'", exp.NodeType));
			}
		}

        /// <summary> Visit member binding </summary>
		protected virtual MemberBinding VisitBinding (MemberBinding binding)
		{
			switch (binding.BindingType)
			{
				case MemberBindingType.Assignment:
					return this.VisitMemberAssignment ((MemberAssignment)binding);
				case MemberBindingType.MemberBinding:
					return this.VisitMemberMemberBinding ((MemberMemberBinding)binding);
				case MemberBindingType.ListBinding:
					return this.VisitMemberListBinding ((MemberListBinding)binding);
				default:
					throw new Exception (string.Format ("Unhandled binding type '{0}'", binding.BindingType));
			}
		}

        /// <summary> Visit element initializer </summary>
		protected virtual ElementInit VisitElementInitializer (ElementInit initializer)
		{
			ReadOnlyCollection<Expression> arguments = this.VisitExpressionList (initializer.Arguments);
			if (arguments != initializer.Arguments)
			{
				return Expression.ElementInit (initializer.AddMethod, arguments);
			}
			return initializer;
		}

        /// <summary> Visit one-parameter expression </summary>
        protected virtual Expression VisitUnary (UnaryExpression u)
		{
			Expression operand = this.Visit (u.Operand);
			if (operand != u.Operand)
			{
				return Expression.MakeUnary (u.NodeType, operand, u.Type, u.Method);
			}
			return u;
		}

        /// <summary> Visit two-parameter expression </summary>
		protected virtual Expression VisitBinary (BinaryExpression b)
		{
			Expression left = this.Visit (b.Left);
			Expression right = this.Visit (b.Right);
			Expression conversion = this.Visit (b.Conversion);
			if (left != b.Left || right != b.Right || conversion != b.Conversion)
			{
				if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
					return Expression.Coalesce (left, right, conversion as LambdaExpression);
				else
					return Expression.MakeBinary (b.NodeType, left, right, b.IsLiftedToNull, b.Method);
			}
			return b;
		}

        /// <summary> Visit type-is expression </summary>
        protected virtual Expression VisitTypeIs (TypeBinaryExpression b)
		{
			Expression expr = this.Visit (b.Expression);
			if (expr != b.Expression)
			{
				return Expression.TypeIs (expr, b.TypeOperand);
			}
			return b;
		}

        /// <summary> Return constant expression </summary>
		protected virtual Expression VisitConstant (ConstantExpression c)
		{
			return c;
		}

        /// <summary> Simplify conditional expression </summary>
		protected virtual Expression VisitConditional (ConditionalExpression c)
		{
			Expression test = this.Visit (c.Test);
			Expression ifTrue = this.Visit (c.IfTrue);
			Expression ifFalse = this.Visit (c.IfFalse);
			if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
			{
				return Expression.Condition (test, ifTrue, ifFalse);
			}
			return c;
		}

        /// <summary> Return parameter expression </summary>
        protected virtual Expression VisitParameter (ParameterExpression p)
		{
			return p;
		}

        /// <summary> Visit member access </summary>
        protected virtual Expression VisitMemberAccess (MemberExpression m)
		{
			Expression exp = this.Visit (m.Expression);
			if (exp != m.Expression)
			{
				return Expression.MakeMemberAccess (exp, m.Member);
			}
			return m;
		}

        /// <summary> Visit method call </summary>
		protected virtual Expression VisitMethodCall (MethodCallExpression m)
		{
			Expression obj = this.Visit (m.Object);
			IEnumerable<Expression> args = this.VisitExpressionList (m.Arguments);
			if (obj != m.Object || args != m.Arguments)
			{
				return Expression.Call (obj, m.Method, args);
			}
			return m;
		}

        /// <summary> Visit list of expressions </summary>
		protected virtual ReadOnlyCollection<Expression> VisitExpressionList (ReadOnlyCollection<Expression> original)
		{
			List<Expression> list = null;
			for (int i = 0, n = original.Count; i < n; i++)
			{
				Expression p = this.Visit (original [i]);
				if (list != null)
				{
					list.Add (p);
				}
				else if (p != original [i])
				{
					list = new List<Expression> (n);
					for (int j = 0; j < i; j++)
					{
						list.Add (original [j]);
					}
					list.Add (p);
				}
			}
			if (list != null)
			{
				return list.AsReadOnly ();
			}
			return original;
		}

        /// <summary> Visit member assignment </summary>
        protected virtual MemberAssignment VisitMemberAssignment (MemberAssignment assignment)
		{
			Expression e = this.Visit (assignment.Expression);
			if (e != assignment.Expression)
			{
				return Expression.Bind (assignment.Member, e);
			}
			return assignment;
		}

        /// <summary> Visit member binding </summary>
		protected virtual MemberMemberBinding VisitMemberMemberBinding (MemberMemberBinding binding)
		{
			IEnumerable<MemberBinding> bindings = this.VisitBindingList (binding.Bindings);
			if (bindings != binding.Bindings)
			{
				return Expression.MemberBind (binding.Member, bindings);
			}
			return binding;
		}

        /// <summary> Visit member list binding </summary>
        protected virtual MemberListBinding VisitMemberListBinding (MemberListBinding binding)
		{
			IEnumerable<ElementInit> initializers = this.VisitElementInitializerList (binding.Initializers);
			if (initializers != binding.Initializers)
			{
				return Expression.ListBind (binding.Member, initializers);
			}
			return binding;
		}

        /// <summary> Visit list of bindings </summary>
		protected virtual IEnumerable<MemberBinding> VisitBindingList (ReadOnlyCollection<MemberBinding> original)
		{
			List<MemberBinding> list = null;
			for (int i = 0, n = original.Count; i < n; i++)
			{
				MemberBinding b = this.VisitBinding (original [i]);
				if (list != null)
				{
					list.Add (b);
				}
				else if (b != original [i])
				{
					list = new List<MemberBinding> (n);
					for (int j = 0; j < i; j++)
					{
						list.Add (original [j]);
					}
					list.Add (b);
				}
			}
			if (list != null)
				return list;
			return original;
		}

        /// <summary> Visit list of element-initializers </summary>
        protected virtual IEnumerable<ElementInit> VisitElementInitializerList (ReadOnlyCollection<ElementInit> original)
		{
			List<ElementInit> list = null;
			for (int i = 0, n = original.Count; i < n; i++)
			{
				ElementInit init = this.VisitElementInitializer (original [i]);
				if (list != null)
				{
					list.Add (init);
				}
				else if (init != original [i])
				{
					list = new List<ElementInit> (n);
					for (int j = 0; j < i; j++)
					{
						list.Add (original [j]);
					}
					list.Add (init);
				}
			}
		    return list != null ? (IEnumerable<ElementInit>) list : original;
		}

        /// <summary> Visit lambda expression </summary>
        protected virtual Expression VisitLambda (LambdaExpression lambda)
		{
			Expression body = this.Visit (lambda.Body);
			return body != lambda.Body ? Expression.Lambda (lambda.Type, body, lambda.Parameters) : lambda;
		}

        /// <summary> Visit new-expression </summary>
		protected virtual NewExpression VisitNew (NewExpression nex)
		{
		    IEnumerable<Expression> args = this.VisitExpressionList (nex.Arguments);
		    return args != nex.Arguments
		               ? (nex.Members != null
		                      ? Expression.New(nex.Constructor, args, nex.Members)
		                      : Expression.New(nex.Constructor, args))
		               : nex;
		}

        /// <summary> Visit member init expression </summary>
	    protected virtual Expression VisitMemberInit (MemberInitExpression init)
		{
			NewExpression n = this.VisitNew (init.NewExpression);
			IEnumerable<MemberBinding> bindings = this.VisitBindingList (init.Bindings);
	        return n != init.NewExpression || bindings != init.Bindings ? Expression.MemberInit(n, bindings) : init;
		}

        /// <summary> Visit list init expression </summary>
		protected virtual Expression VisitListInit (ListInitExpression init)
		{
			NewExpression n = this.VisitNew (init.NewExpression);
			IEnumerable<ElementInit> initializers = this.VisitElementInitializerList (init.Initializers);
		    return n != init.NewExpression || initializers != init.Initializers ? Expression.ListInit(n, initializers) : init;
		}

        /// <summary> Visit array expression </summary>
        protected virtual Expression VisitNewArray (NewArrayExpression na)
		{
		    IEnumerable<Expression> exprs = this.VisitExpressionList (na.Expressions);
		    return exprs != na.Expressions
		               ? (na.NodeType == ExpressionType.NewArrayInit
		                      ? Expression.NewArrayInit(na.Type.GetElementType(), exprs)
		                      : Expression.NewArrayBounds(na.Type.GetElementType(), exprs))
		               : na;
		}

        /// <summary> Visit invocation expression </summary>
	    protected virtual Expression VisitInvocation (InvocationExpression iv)
		{
			IEnumerable<Expression> args = this.VisitExpressionList (iv.Arguments);
			Expression expr = this.Visit (iv.Expression);
	        return args != iv.Arguments || expr != iv.Expression ? Expression.Invoke(expr, args) : iv;
		}
	}
}
