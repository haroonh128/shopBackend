namespace Shop.Core.DTOs
{
    public class SecurityStatusResponse
    {
        public bool IsTwoFactorEnabled { get; set; }
        public int ActiveSessions { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastPasswordChange { get; set; }
    }
}
