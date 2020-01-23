using System.Linq;
using LinqKit;

namespace GenericClassLibrary
{
    public class GenericClass
    {
        public void X()
        {
            var set2 = new[] { 1, 2, 3 }.AsQueryable();
            set2.AsExpandable();
        }
    }
}