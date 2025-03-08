using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace SpaceSoftSolutions.Service
{

    // var hashPass = PasswordHasher.HashPassword(RegesterUser.Password); // Generate hash and salt

    public static class PasswordHasher
    {
        private static readonly string salt = "BDJqgYiuQtPCP4YNzYjSHg==";
        public static string HashPassword(string password)
        {
            // Generate a random salt
            byte[] saltBytes = Convert.FromBase64String(salt);


            // Generate the hash
            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hash;
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Convert the stored salt back to bytes
            byte[] saltBytes = Convert.FromBase64String(salt);

            // Hash the entered password with the same salt
            string enteredHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Compare the hashes
            return storedHash == enteredHash;
        }
    }
}
