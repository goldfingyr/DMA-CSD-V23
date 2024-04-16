using System.Security.Cryptography;
using System.Text;

namespace Cryptography.Ciphers
{
    public class DES : ICipher
    {
        private DESCryptoServiceProvider des = new DESCryptoServiceProvider();

        public DES()
        {
            des.GenerateKey();
            des.GenerateIV();
        }

        public byte[] Cipher(string plainText)
        {
            byte[] encryptedData;

            ICryptoTransform encryptor = des.CreateEncryptor();

            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            encryptedData = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            return encryptedData;
        }

        public string DeCipher(byte[] cipherBytes)
        {
            byte[] decryptedData;

            ICryptoTransform decryptor = des.CreateDecryptor();

            decryptedData = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(decryptedData);
        }

        public List<string> GetKey()
        {
            List<string> keys = new List<string>();
            keys.Add(Convert.ToBase64String(des.Key));
            keys.Add(Convert.ToBase64String(des.IV));
            return keys;
        }

        public void SetKey(List<string> theKeys)
        {
            des.Key = Convert.FromBase64String(theKeys[0]);
            des.IV = Convert.FromBase64String(theKeys[1]);
        }
    }
}
