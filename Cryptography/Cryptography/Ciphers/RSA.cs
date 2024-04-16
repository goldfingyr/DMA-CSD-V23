
using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;

namespace Cryptography.Ciphers
{
    public class RSA : ICipher
    {
        private RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        private List<string> keys = new List<string>();
        public byte[] Cipher(string plainText)
        {
            // fOAEP determines which padding to use
            // Will use Public Key ([0]) and fOAEP = false
            byte[] encryptedData;
            bool fOAEP = false;

            rsa.ImportParameters(JsonConvert.DeserializeObject<RSAParameters>(keys[0]));
            encryptedData = rsa.Encrypt(Encoding.UTF8.GetBytes(plainText), fOAEP);

            return encryptedData;
        }

        public string DeCipher(byte[] cipherBytes)
        {
            // Will use Public Key ([1]) and fOAEP = False
            byte[] decryptedData;
            bool fOAEP = false;

            rsa.ImportParameters(JsonConvert.DeserializeObject<RSAParameters>(keys[1]));
            decryptedData = rsa.Decrypt(cipherBytes, fOAEP);

            return Encoding.UTF8.GetString(decryptedData);
        }

        public List<string> GetKey()
        {
            // Public Key
            keys.Add(JsonConvert.SerializeObject(rsa.ExportParameters(false)));
            // Private Key
            keys.Add(JsonConvert.SerializeObject(rsa.ExportParameters(true)));
            return keys;
        }

        public void SetKey(List<string> theKeys)
        {
            keys = theKeys;
        }
    }
}
