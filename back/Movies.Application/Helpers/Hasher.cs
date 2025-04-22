using System.Security.Cryptography;
using System.Text;

namespace Movies.Application.Helpers;

public class Hasher
{
    public static string HashPassword(string pass)
    {
        if (string.IsNullOrEmpty(pass))
        {
            throw new ArgumentException("Password cannot be null or empty", nameof(pass));
        }

        using (var sha256 = SHA256.Create())
        {
            // Convert the input string to a byte array
            byte[] passBytes = Encoding.UTF8.GetBytes(pass);
            
            // Compute the hash
            byte[] hashBytes = sha256.ComputeHash(passBytes);

            // Convert the hash bytes to a hexadecimal string
            StringBuilder hashBuilder = new StringBuilder();
            foreach (var b in hashBytes)
            {
                hashBuilder.Append(b.ToString("x2")); // Convert to hexadecimal format
            }

            return hashBuilder.ToString();
        }
    }
}