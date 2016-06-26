#if !(NET35)
using System;
using System.Threading.Tasks;

namespace LinqKit.Utitilies
{
    internal static class TaskHelper
    {
        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
#if NET40 || PORTABLE40 || SILVERLIGHT
            return Task.Factory.StartNew<TResult>(function);
#else
            return Task.Run(function);
#endif
        }
    }
}
#endif