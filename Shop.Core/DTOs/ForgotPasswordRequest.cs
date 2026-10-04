namespace Shop.Core.DTOs
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public string NewPin { get; set; } = string.Empty;
        public string ConfirmNewPin { get; set; } = string.Empty;
    }
}
