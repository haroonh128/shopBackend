namespace Shop.Core.DTOs
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsTwoFactorEnabled { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
