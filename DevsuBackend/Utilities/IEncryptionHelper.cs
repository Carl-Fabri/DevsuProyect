namespace DevsuBackend.Utilities
{
    public interface IEncryptionHelper
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
