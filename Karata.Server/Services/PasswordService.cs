using Konscious.Security.Cryptography;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Karata.Server.Services
{
    public class PasswordService : IPasswordService
    {
        public bool VerifyHash(byte[] password, byte[] salt, byte[] hash) =>
            hash.SequenceEqual(HashPassword(password, ref salt));

        public byte[] HashPassword(byte[] password, ref byte[] salt)
        {
            salt ??= CreateSalt();

            using var argon2 = new Argon2id(password);
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 8;
            argon2.Iterations = 4;
            argon2.MemorySize = 1024 * 128;

            return argon2.GetBytes(16);
        }

        private byte[] CreateSalt()
        {
            var buffer = new byte[16];
            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }
    }
}
