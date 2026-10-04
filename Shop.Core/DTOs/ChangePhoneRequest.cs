namespace Shop.Core.DTOs
{
    public class ChangePhoneRequest
    {
        public string NewPhoneNumber { get; set; } = string.Empty;
        public string Pin { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
