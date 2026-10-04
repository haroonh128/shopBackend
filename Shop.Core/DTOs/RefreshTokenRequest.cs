
namespace Shop.Core.DTOs
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class TwoFactorToggleRequest
    {
        public bool Enable { get; set; }
    }
}
