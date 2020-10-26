namespace ConsoleAppLinqBug
{
    public class AuditDocument<T>
    {
        public T Entity { get; set; }

        public bool Active { get; set; }

        public AuditDocument(T entity, bool active)
        {
            Entity = entity;
            Active = active;
        }
    }
}