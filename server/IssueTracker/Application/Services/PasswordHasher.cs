using System;
using System.Security.Cryptography;
using System.Text;

namespace IssueTracker.Application.Services
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Convert the input string to a byte array and compute the hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static bool VerifyPassword(string input, string hash)
        {
            var hashedInput = HashPassword(input);
            
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(hashedInput, hash) == 0;
        }
    }
}
