namespace Shop.Entities
{
    public class User: Base
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string PinHash { get; set; } = string.Empty;
        public bool IsTwoFactorEnabled { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
