namespace Shop.Entities
{
    public class OtpCode : Base
    {
        public Guid UserId { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedAt { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public User User { get; set; } = null!;
    }
}
