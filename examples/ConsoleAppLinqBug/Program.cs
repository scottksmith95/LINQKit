using System;

namespace ConsoleAppLinqBug
{
    public class Program
    {
        static void Main(string[] args)
        {
            var repository = new Repository();

            var doc = repository.GetDocument(sd => sd.Id == new Guid("08d87544-4aae-8e73-afd5-772ab0a086a1"));
            bool x = doc != null;
            Console.WriteLine(x);
            
            var nullDoc = repository.GetDocument(sd => sd.Id == new Guid("08d8754a-fb8f-8fe8-afd5-772770f5e423"));
            Console.WriteLine(nullDoc is null);
        }
    }
}
