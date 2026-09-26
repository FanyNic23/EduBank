using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace EduBank.AppWeb.Helpers
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Generar un salt aleatorio
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Derivar la contraseña hash
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Combinar salt y hash para almacenamiento
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                // Separar salt y hash almacenados
                var parts = storedHash.Split('.');
                var salt = Convert.FromBase64String(parts[0]);
                var storedSubHash = parts[1];

                // Hashear la contraseña proporcionada con el mismo salt
                var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                return hashed == storedSubHash;
            }
            catch
            {
                return false;
            }
        }
    }
}
