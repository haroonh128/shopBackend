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

        /// <summary>
        /// The other spelling of the same Pakistani number.
        /// 03001234567 and +923001234567 are alternates of each other.
        /// Returns null when the input is not a valid number or the swap would fall outside 9–13 digits.
        /// </summary>
        public static string? AlternateForm(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) || !IsValid(phoneNumber))
                return null;

            string alternate;
            if (phoneNumber.StartsWith("03", StringComparison.Ordinal))
                alternate = "+92" + phoneNumber[1..];
            else if (phoneNumber.StartsWith("+92", StringComparison.Ordinal))
                alternate = "0" + phoneNumber[3..];
            else
                return null;

            return IsValid(alternate) ? alternate : null;
        }

        /// <summary>
        /// The number as typed, plus its local/international alternate when that form is also valid.
        /// </summary>
        public static IReadOnlyList<string> EquivalentForms(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return Array.Empty<string>();

            var alternate = AlternateForm(phoneNumber);
            if (alternate == null)
                return new[] { phoneNumber };

            return new[] { phoneNumber, alternate };
        }
    }
}
