

namespace Shop.Core.DTOs
{
    public class TwoFactorStatusResponse
    {
        public bool IsEnabled { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
