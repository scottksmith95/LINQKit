namespace LinqKit.Microsoft.EntityFrameworkCore.Tests.Model
{
    public class TestEntity
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Value { get; set; }
    }
}