namespace Karata.Server.Services
{
    public interface IPasswordService
    {
        byte[] HashPassword(byte[] password, ref byte[] salt);
        bool VerifyHash(byte[] password, byte[] salt, byte[] hash);
    }
}