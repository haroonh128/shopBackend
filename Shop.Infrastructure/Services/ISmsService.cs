
namespace Shop.Infrastructure.Services
{
    public interface ISmsService
    {
        Task<bool> SendOtpAsync(string phoneNumber, string otpCode);
    }
}