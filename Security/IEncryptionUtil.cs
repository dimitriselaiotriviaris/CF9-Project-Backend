namespace CF9Project.Security
{
    public interface IEncryptionUtil
    {
        string Encrypt(string clearText);
        bool IsValidPassword(string plainText, string cipherText);
    }
}
