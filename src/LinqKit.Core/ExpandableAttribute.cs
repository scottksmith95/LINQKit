using System;
using JetBrains.Annotations;

namespace LinqKit
{
    /// <summary>
    /// When applied to method or property, tells LINQKit to replace them in queryable LINQ expression with another expression,
    /// returned by method, specified in this attribute.
    ///
    /// Requirements to expression method:
    /// <para>
    /// - expression method should be in the same class and replaced property of method;
    /// - method could be private.
    /// </para>
    /// <para>
    /// When applied to property, expression:
    /// - method should return function expression with the same return type as property type;
    /// - expression method could take one parameter - current object parameter.
    /// </para>
    /// <para>
    /// When applied to method:
    /// - expression method should return function expression with the same return type as method return type;
    /// - method cannot have void return type;
    /// - parameters in expression method should go in the same order as in substituted method;
    /// - expression could take method instance object as first parameter;
    /// </para>
    /// </summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ExpandableAttribute : Attribute
    {
        /// <summary>
        /// Creates instance of attribute.
        /// </summary>
        /// <param name="methodName">Name of method in the same class that returns substitution expression. [Optional]</param>
        public ExpandableAttribute(string methodName = null)
        {
            MethodName = methodName;
        }

        /// <summary>
        /// Name of method in the same class that returns substitution expression.
        /// </summary>
        public string MethodName { get; set; }
    }
}