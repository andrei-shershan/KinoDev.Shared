using System;
using System.Security.Cryptography;
using System.Text;

namespace KinoDev.Shared.Helpers
{
    public static class HashHelper
    {
        /// <summary>
        /// Calculates SHA256 hash from input string with salt and returns the result as a hexadecimal string
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <param name="salt">Salt to add to the input before hashing</param>
        /// <returns>Hexadecimal string representation of the hash</returns>
        public static string CalculateSha256Hash(string input, string salt)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input), "Input string cannot be null or empty");
            }

            if (salt == null)
            {
                salt = string.Empty;
            }

            // Combine input and salt
            string combined = input + salt;
            
            // Convert the combined string to bytes
            byte[] inputBytes = Encoding.UTF8.GetBytes(combined);
            
            // Calculate hash
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                
                // Convert hash bytes to a hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                
                return sb.ToString();
            }
        }
    }
}