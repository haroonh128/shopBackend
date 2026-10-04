using System.Text.RegularExpressions;

namespace Shop.Common
{
    public static class PhoneNumberValidator
    {
        public static bool IsValid(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Basic validation: starts with +, followed by 10-15 digits
            var regex = new Regex(@"^\+[1-9]\d{9,14}$");
            return regex.IsMatch(phoneNumber);
        }
    }
}
