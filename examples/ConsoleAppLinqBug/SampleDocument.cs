using System;

namespace ConsoleAppLinqBug
{
    public class SampleDocument
    {
        public Guid Id { get; set; }

        public SampleDocument(Guid id)
        {
            Id = id;
        }
    }
}