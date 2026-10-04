namespace Shop.Entities
{
    public class AuditLog : Base
    {
        public Guid? UserId { get; set; }
        public string EntityName { get; set; } = null;
        public string Action { get; set; } = null; // Create, Update, Delete
        public string? OldValues { get; set; } = null;
        public string? NewValues { get; set; } = null;
        public string? IpAddress { get; set; } = null;
        public string? UserAgent { get; set; } = null;
        public User? User { get; set; }
    }
}
