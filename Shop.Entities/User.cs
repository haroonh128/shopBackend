namespace Shop.Entities
{
    public class User: Base
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string PinHash { get; set; } = string.Empty;
        public bool IsTwoFactorEnabled { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string CNIC { get; set; } = string.Empty;
        public string AppType { get; set; } = string.Empty;
        public Guid? ModuleId { get; set; }
        /// <summary>Platform administrator — bypasses subscription validity checks.</summary>
        public bool IsAdmin { get; set; }
        /// <summary>Login allowed through end of this UTC date when subscription is paid.</summary>
        public DateTime? SubscriptionValidUntil { get; set; }
    }
}
