
namespace LinqKit.EntityFramework.Tests
{
    public class Entity
    {
        public int Id { get; set; }

        public int Value { get; set; }

        public RelatedEntity RelatedEntity { get; set; }
    }

    public class RelatedEntity
    {
        public int Id { get; set; }

        public int Value { get; set; }
    }
}