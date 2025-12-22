namespace Shop.Core.DTOs
{
    public class ChangePinRequest
    {
        public string CurrentPin { get; set; } = string.Empty;
        public string NewPin { get; set; } = string.Empty;
        public string ConfirmNewPin { get; set; } = string.Empty;
    }
}
