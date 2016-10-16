namespace LinqKit.Core.Utilities
{
    internal static class Util
    {
        private const string ExceptionMessage =
            "This NuGet package is obsolete.\r\n" +
            "* Use LinqKit.Core if you want to use the functionality without a reference to any EntityFramework.\r\n" +
            "* Use LinqKit.EntityFramework if you are using this library together with EntityFramework version 6.3.\r\n" +
            "* Use LinqKit.Microsoft.EntityFrameworkCore if you are using this library together with Microsoft.EntityFrameworkCore version 1.x.x.";

        public static void IsSupported()
        {
#if THROWEXONNOTSUPPORTED
            throw new System.NotSupportedException(ExceptionMessage);
#endif
        }
    }
}