using System.Security.Cryptography;

namespace Cryptography.Ciphers
{
    public class DES3 : ICipher
    {
        private TripleDES des3 = TripleDES.Create();

        public byte[] Cipher(string plainText)
        {
            byte[] encrypted;


            // Create a new MemoryStream object to contain the encrypted bytes.
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Create a CryptoStream object to perform the encryption.
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, des3.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    // Encrypt the plaintext.
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }

                    encrypted = memoryStream.ToArray();
                }
            }

            return encrypted;
        }

        public string DeCipher(byte[] cipherBytes)
        {
            string decrypted;

            // Create a new MemoryStream object to contain the decrypted bytes.
            using (MemoryStream memoryStream = new MemoryStream(cipherBytes))
            {
                // Create a CryptoStream object to perform the decryption.
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, des3.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    // Decrypt the ciphertext.
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        decrypted = streamReader.ReadToEnd();
                    }
                }
            }

            return decrypted;
        }

        public List<string> GetKey()
        {
            List<string> keys = new List<string>();
            keys.Add(Convert.ToBase64String(des3.Key));
            keys.Add(Convert.ToBase64String(des3.IV));
            return keys;
        }

        public void SetKey(List<string> theKeys)
        {
            des3.Key = Convert.FromBase64String(theKeys[0]);
            des3.IV = Convert.FromBase64String(theKeys[1]);
        }
    }
}
