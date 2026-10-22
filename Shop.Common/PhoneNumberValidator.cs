using System.Text.RegularExpressions;

namespace Shop.Common
{
    public static class PhoneNumberValidator
    {
        /// <summary>
        /// Pakistani phone numbers: +92... or 03..., with 9–13 digits total.
        /// Examples: +923001234567, 03001234567
        /// </summary>
        private static readonly Regex PhonePattern = new(@"^(\+92|03)\d*$", RegexOptions.Compiled);

        public static bool IsValid(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            if (!PhonePattern.IsMatch(phoneNumber))
                return false;

            var digitCount = 0;
            foreach (var c in phoneNumber)
            {
                if (char.IsDigit(c))
                    digitCount++;
            }

            return digitCount >= 9 && digitCount <= 13;
        }
    }
}
