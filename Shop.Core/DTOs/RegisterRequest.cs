
namespace Shop.Core.DTOs
{
    public class RegisterRequest
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string Pin { get; set; } = string.Empty;
        public string ConfirmPin { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CNIC { get; set; } = string.Empty;
        public bool TwoFactorAuthentication { get; set; } = false;
        public string AppType { get; set; } = string.Empty;
    }
}
