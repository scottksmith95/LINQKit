using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace LinqKit.Tests.Net452
{
    public class ExpressionExpanderTests
    {
        [Fact]
        public void ExpressionCombiner_Expression_Subtract()
        {
            var exp = Expression.Subtract(Expression.Constant(5), Expression.Constant(2));
            var executed = exp.Expand().ToString();
            Assert.Equal(exp.ToString(), executed);
        }

        [Fact]
        public void ExpressionCombiner_Expression_Power()
        {
            var exp = Expression.Power(Expression.Constant(5.1), Expression.Constant(2.5));
            var executed = exp.Expand().ToString();
            Assert.Equal(exp.ToString(), executed);
        }

        [Fact]
        public void ExpressionCombiner_Expression_UnaryPlus()
        {
            var exp = Expression.UnaryPlus(Expression.Constant(5));
            var executed = exp.Expand().ToString();
            Assert.Equal(exp.ToString(), executed);
        }

        [Fact]
        public void ExpressionExpander_Expression_Index()
        {
            var listParameter = Expression.Parameter(typeof(List<string>), "l");
            Expression<Func<List<string>, string>> expression1 = Expression.Lambda<Func<List<string>, string>>(Expression.MakeIndex(
                    listParameter,
                    typeof(List<string>).GetProperties().SingleOrDefault(p => p.GetIndexParameters().Length > 0),
                    new[] { Expression.Constant(0) }),
                listParameter);
            Expression<Func<string, string>> expression2 = s => s;

            var executed = Linq.Expr((List<string> l) => expression2.Invoke(expression1.Invoke(l))).Expand().ToString();
            Assert.Equal(expression1.ToString(), executed);
        }

        [Fact]
        public void ExpressionExpander_Expression_Block()
        {
            var objParameter = Expression.Parameter(typeof(object), "o");
            var objVar = Expression.Variable(typeof(object));
            
            var lambda = Expression.Lambda<Func<object, string>>(Expression.Block(new[] {objVar},
                    Expression.Assign(objVar, objParameter),
                    Expression.Call(objVar, nameof(ToString), Type.EmptyTypes)),
                objParameter);

            var expandedLambda = Linq.Expr((object o) => lambda.Invoke(o))
                .Expand();
            Assert.Equal(lambda.ToString(), expandedLambda.ToString());
            Assert.Equal(lambda.Invoke(42), expandedLambda.Invoke(42));
        }

        [Fact]
        public void ExpressionExpander_Expression_InvokeExpressionRemoved()
        {
            Expression<Func<object, object>> lambda = o => o;

            var expandedLambda = Linq.Expr((object o) => lambda.Invoke(o))
                .Expand();
            Assert.Equal(ExpressionType.Parameter, expandedLambda.Body.NodeType);
            Assert.Equal(lambda.ToString(), expandedLambda.ToString());
            Assert.Equal(lambda.Invoke(42), expandedLambda.Invoke(42));
        }

        [Fact]
        public void ExpressionExpander_Expression_CompileAndInvokeExpressionRemoved()
        {
            Expression<Func<object, object>> lambda = o => o;

            var expandedLambda = Linq.Expr((object o) => lambda.Compile()(o))
                .Expand();
            Assert.Equal(ExpressionType.Parameter, expandedLambda.Body.NodeType);
            Assert.Equal(lambda.ToString(), expandedLambda.ToString());
            Assert.Equal(lambda.Invoke(42), expandedLambda.Invoke(42));
        }

        [Fact]
        public void ExpressionExpander_Expression_CompileAndInvokeOnExpressionRemoved()
        {
            Expression<Func<object, object>> lambda = o => o;

            var expandedLambda = Linq.Expr((object o) => lambda.Compile().Invoke(o))
                .Expand();
            Assert.Equal(ExpressionType.Parameter, expandedLambda.Body.NodeType);
            Assert.Equal(lambda.ToString(), expandedLambda.ToString());
            Assert.Equal(lambda.Invoke(42), expandedLambda.Invoke(42));
        }

        [Fact]
        public void ExpressionExpander_Expression_InvokeDelegate()
        {
            Func<object, string> func = o => o.ToString();

            Expression<Func<object, string>> lambda = o => func(o);

            var expandedLambda = Linq.Expr((object o) => lambda.Invoke(o))
                .Expand();
            Assert.Equal(lambda.ToString(), expandedLambda.ToString());
            Assert.Equal(lambda.Invoke(42), expandedLambda.Invoke(42));
        }

        [Fact]
        public void ExpressionExpander_Expression_InvokeOnDelegate()
        {
            Func<object, string> func = o => o.ToString();

            Expression<Func<object, string>> lambda = o => func.Invoke(o);

            var expandedLambda = Linq.Expr((object o) => lambda.Invoke(o))
                .Expand();
            Assert.Equal(lambda.ToString(), expandedLambda.ToString());
            Assert.Equal(lambda.Invoke(42), expandedLambda.Invoke(42));
        }

        [Fact]
        public void ExpressionExpander_Expression_Throw()
        {
            var objParameter = Expression.Parameter(typeof(object), "o");
            var msgParameter = Expression.Parameter(typeof(string), "msg");

            var exceptionConstructor = typeof(ArgumentNullException).GetConstructor(new []{typeof(string)});

            var lambda = Expression.Lambda<Func<object, string, object>>(
                Expression.Condition(
                    Expression.Equal(objParameter, Expression.Constant(null)),
                    Expression.Throw(Expression.New(exceptionConstructor, msgParameter), typeof(object)),
                    objParameter),
                objParameter,
                msgParameter);

            var expandedLambda = Linq.Expr((object o, string msg) => lambda.Invoke(o, msg))
                .Expand();
            Assert.Equal(lambda.ToString(), expandedLambda.ToString());
            Assert.Equal("x",
                Assert.Throws<ArgumentNullException>(() => lambda.Invoke(null, "x"))
                    .ParamName);
            Assert.Equal("x",
                Assert.Throws<ArgumentNullException>(() => expandedLambda.Invoke(null, "x"))
                    .ParamName);
            var obj = new object();
            Assert.Same(lambda.Invoke(obj, "x"), expandedLambda.Invoke(obj, "x"));
        }
    }
}
