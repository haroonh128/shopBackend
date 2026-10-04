namespace Shop.Core.Interfaces.Services
{
    public interface IEmailService
    {
        Task<bool> SendOtpAsync(string toEmail, string otpCode);
    }
}
