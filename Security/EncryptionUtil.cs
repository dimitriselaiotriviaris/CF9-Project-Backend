namespace SchoolApp.Security
{
    public class EncryptionUtil : IEncryptionUtil
    {
        public string Encrypt(string clearText)
        {
            return BCrypt.Net.BCrypt.HashPassword(clearText);
        }

        public bool IsValidPassword(string plainText, string cipherText)
        {
            return BCrypt.Net.BCrypt.Verify(plainText, cipherText);
        }
    }
}
