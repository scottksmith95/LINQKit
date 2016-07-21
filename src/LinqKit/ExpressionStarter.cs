using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
#if !(NET35) && !(WINDOWS_APP) && !(NETSTANDARD) && !(PORTABLE40) && !(UAP)
using System.Reflection.Emit;
#endif
using System.Runtime.CompilerServices;

namespace LinqKit
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExpressionStarter<T>
    {
        internal ExpressionStarter() { }

        internal ExpressionStarter(bool defaultExpression)
        {
            if (defaultExpression)
                DefaultExpression = f => true;
            else
                DefaultExpression = f => false;
        }

        internal ExpressionStarter(Expression<Func<T, bool>> exp) { _Predicate = exp; }

        /// <summary>The actual Predicate. It can only be set by calling Start.</summary>
        private Expression<Func<T, bool>> Predicate
        {
            get { return (IsStarted || !UseDefaultExpression) ? _Predicate : DefaultExpression; }
        }
        private Expression<Func<T, bool>> _Predicate;

        /// <summary>Determines if the predicate is started.</summary>
        public bool IsStarted { get { return _Predicate != null; } }

        /// <summary> A default expression to use only when the expression is null </summary>
        public bool UseDefaultExpression { get { return DefaultExpression != null; } }

        /// <summary>The default expression</summary>
        public Expression<Func<T, bool>> DefaultExpression { get; set; }

        /// <summary>Set the Expression predicate</summary>
        /// <param name="exp">The first expression</param>
        public Expression<Func<T, bool>> Start(Expression<Func<T, bool>> exp)
        {
            if (IsStarted)
                throw new Exception("Predicate cannot be started again.");
            return (_Predicate = exp);
        }

        /// <summary> OR </summary>
        public Expression<Func<T, bool>> Or(Expression<Func<T, bool>> expr2)
        {
            return (IsStarted) ? _Predicate = Predicate.Or(expr2) : Start(expr2);
        }

        /// <summary> OR </summary>
        public Expression<Func<T, bool>> And(Expression<Func<T, bool>> expr2)
        {
            return (IsStarted) ? _Predicate = Predicate.And(expr2) : Start(expr2);
        }

        /// <summary> Show predicate string </summary>
        public override string ToString()
        {
            return Predicate == null ? null : Predicate.ToString();
        }

        #region Implicit Operators

        /// <summary>
        /// Allows this object to be implicitely converted to an Expression{Func{T, bool}}.
        /// </summary>
        /// <param name="right"></param>
        public static implicit operator Expression<Func<T, bool>>(ExpressionStarter<T> right)
        {
            return right == null ? null : right.Predicate;
        }

        /// <summary>
        /// Allows this object to be implicitely converted to an Expression{Func{T, bool}}.
        /// </summary>
        /// <param name="right"></param>
        public static implicit operator Func<T, bool>(ExpressionStarter<T> right)
        {
            return right == null ? null : (right.IsStarted || right.UseDefaultExpression) ? right.Predicate.Compile() : null;
        }

        /// <summary>
        /// Allows this object to be implicitely converted to an Expression{Func{T, bool}}.
        /// </summary>
        /// <param name="right"></param>
        public static implicit operator ExpressionStarter<T>(Expression<Func<T, bool>> right)
        {
            return right == null ? null : new ExpressionStarter<T>(right);
        }

        #endregion

        #region Implement Expression<TDelagate> methods and properties

#if !(NET35)

        /// <summary></summary>
        public Func<T, bool> Compile() { return Predicate.Compile(); }
#endif
#if !(NET35) && !(WINDOWS_APP) && !(NETSTANDARD) && !(PORTABLE40) && !(UAP)

        /// <summary></summary>
        public Func<T, bool> Compile(DebugInfoGenerator debugInfoGenerator) { return Predicate.Compile(debugInfoGenerator); }
                
        /// <summary></summary>
        public Expression<Func<T, bool>> Update(Expression body, IEnumerable<ParameterExpression> parameters) { return Predicate.Update(body, parameters); }

#endif

        #endregion

        #region Implement LamdaExpression methods and properties

        /// <summary></summary>
        public Expression Body { get { return Predicate.Body; } }


        /// <summary></summary>
        public ExpressionType NodeType { get { return Predicate.NodeType; } }

        /// <summary></summary>
        public ReadOnlyCollection<ParameterExpression> Parameters { get { return Predicate.Parameters; } }


        /// <summary></summary>
        public Type Type { get { return Predicate.Type; } }

#if !(NET35)

        /// <summary></summary>
        public string Name { get { return Predicate.Name; } }
        
        /// <summary></summary>
        public Type ReturnType { get { return Predicate.ReturnType; } }

        /// <summary></summary>
        public bool TailCall { get { return Predicate.TailCall; } }
#endif
#if !(NET35) && !(WINDOWS_APP) && !(NETSTANDARD) && !(PORTABLE40) && !(UAP)
        /// <summary></summary>
        public void CompileToMethod(MethodBuilder method) { Predicate.CompileToMethod(method); }
        
        /// <summary></summary>
        public void CompileToMethod(MethodBuilder method, DebugInfoGenerator debugInfoGenerator) { Predicate.CompileToMethod(method, debugInfoGenerator); }

#endif

        #endregion

        #region Implement Expression methods and properties

#if !(NET35)
        
        /// <summary></summary>
        public virtual bool CanReduce { get { return Predicate.CanReduce; } }

#endif
        #endregion
    }
}
