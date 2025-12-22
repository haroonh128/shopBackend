using System.Security.Cryptography;

namespace Shop.Common.Helpers
{
    public static class OtpGenerator
    {
        public static string GenerateOtp(int length)
        {
            if (length <= 0 || length > 10)
            {
                throw new ArgumentException("OTP length must be between 1 and 10", nameof(length));
            }

            // Generate a random number with the specified length
            var min = (int)Math.Pow(10, length - 1);
            var max = (int)Math.Pow(10, length) - 1;
            
            var randomNumber = RandomNumberGenerator.GetInt32(min, max + 1);
            
            return randomNumber.ToString().PadLeft(length, '0');
        }
    }
}

