using System;
using System.Threading.Tasks;

namespace LinqKit.Utilities
{
    internal static class TaskHelper
    {
        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            return Task.Run(function);
        }
    }
}
