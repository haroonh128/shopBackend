namespace Shop.Core.DTOs
{
    public class AdminUserResponse
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string CNIC { get; set; } = string.Empty;
        public string AppType { get; set; } = string.Empty;
        public Guid? ModuleId { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime? SubscriptionValidUntil { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsSubscriptionExpired { get; set; }
    }

    public class AdminCreateUserRequest
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string Pin { get; set; } = string.Empty;
        public string ConfirmPin { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CNIC { get; set; } = string.Empty;
        public string AppType { get; set; } = string.Empty;
        public Guid? ModuleId { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsAdmin { get; set; }
        public DateTime? SubscriptionValidUntil { get; set; }
    }

    public class AdminUpdateUserRequest
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Pin { get; set; }
        public string? ConfirmPin { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CNIC { get; set; } = string.Empty;
        public string AppType { get; set; } = string.Empty;
        public Guid? ModuleId { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime? SubscriptionValidUntil { get; set; }
    }

    public class UpdateSubscriptionRequest
    {
        public DateTime SubscriptionValidUntil { get; set; }
    }

    public class SetUserActiveRequest
    {
        public bool IsActive { get; set; }
    }
}
