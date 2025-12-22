namespace Shop.Common
{
    public static class PinValidator
    {
        public static bool IsValid(string pin, int requiredLength = 4)
        {
            if (string.IsNullOrWhiteSpace(pin))
                return false;

            if (pin.Length != requiredLength)
                return false;

            return pin.All(char.IsDigit);
        }
    }
}
