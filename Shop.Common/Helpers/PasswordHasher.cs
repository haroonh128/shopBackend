using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Shop.Common.Helpers
{
    public static class PasswordHasher
    {
        private const int SaltSize = 128 / 8;
        private const int HashSize = 256 / 8;
        private const int Iterations = 10000;

        public static string HashPin(string pin)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] hash = KeyDerivation.Pbkdf2(
                password: pin,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: Iterations,
                numBytesRequested: HashSize);

            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPin(string pin, string hashString)
        {
            try
            {
                var parts = hashString.Split('.');
                if (parts.Length != 2) return false;

                var salt = Convert.FromBase64String(parts[0]);
                var storedHash = parts[1];

                byte[] hash = KeyDerivation.Pbkdf2(
                    password: pin,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: Iterations,
                    numBytesRequested: HashSize);

                return Convert.ToBase64String(hash) == storedHash;
            }
            catch
            {
                return false;
            }
        }
    }
}
