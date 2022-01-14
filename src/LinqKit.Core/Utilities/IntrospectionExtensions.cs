#if NET35 || NET40 || PORTABLE40 || PORTABLE || SILVERLIGHT
using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    /// <summary>
    /// https://github.com/castleproject/Core/blob/netcore/src/Castle.Core/Compatibility/IntrospectionExtensions.cs
    /// </summary>
	internal static class IntrospectionExtensions
    {
#if NET35 || NET40 || PORTABLE40 || SILVERLIGHT
        // This allows us to use the new reflection API which separates Type and TypeInfo
        // while still supporting .NET 3.5 and 4.0. This class matches the API of the same
        // class in .NET 4.5+, and so is only needed on .NET Framework versions before that.
        //
        // Return the System.Type for now, we will probably need to create a TypeInfo class
        // which inherits from Type like .NET 4.5+ and implement the additional methods and
        // properties.
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }
#endif

        // This is for portable-net45+win8+wpa81+wp8 (Profile259)
#if PORTABLE
        public static IEnumerable<MethodInfo> GetMethods(this Type someType)
        {
            var ti = someType.GetTypeInfo();
            foreach (var m in ti.DeclaredMethods)
            {
                yield return m;
            }
        }

        public static IEnumerable<ConstructorInfo> GetConstructors(this Type someType)
        {
            var ti = someType.GetTypeInfo();
            foreach (var c in ti.DeclaredConstructors)
            {
                yield return c;
            }
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type someType)
        {
            var ti = someType.GetTypeInfo();
            foreach (var p in ti.DeclaredProperties)
            {
                yield return p;
            }
        }

        public static PropertyInfo GetProperty(this Type someType, string name)
        {
            var ti = someType.GetTypeInfo();
            return ti.DeclaredProperties.FirstOrDefault(p => p.Name == name);
        }
#endif
    }
}
#endif