using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Models.Services
{
    using System.Security.Cryptography;
    using System.Text;

    public class PasswordService : IPwService
    {
        public PasswordService() { }
        public string ComputeHash(string password, string salt)
        {
            // Konfiguration
            const int iterations = 69420; // Sicherheit durch Zeitaufwand
            const int hashByteSize = 32;    // 32 Bytes = 256 Bit = 64 Hex-Zeichen

            // Salt als Byte-Array
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt.ToString());

            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(hashByteSize);

            // In Hex umwandeln
            var builder = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }

}
