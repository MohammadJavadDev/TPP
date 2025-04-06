using System.Security.Cryptography;
using System.Text;

namespace RegistrationApi.Utilities
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Using SHA256 for password hashing
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}