

namespace Shop.Core.DTOs
{
    public class AuthResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public UserInfo? User { get; set; }
    }

    public class UserInfo
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsTwoFactorEnabled { get; set; }
    }
}
