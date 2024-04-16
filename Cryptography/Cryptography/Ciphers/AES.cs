using System.Security.Cryptography;

namespace Cryptography.Ciphers
{
    /// <summary>
    /// Ciphering using the AES symmetrical algorithm
    /// </summary>
    public class AES : ICipher
    {
        private Aes aes = Aes.Create();

        /// <summary>
        /// Returns current key
        /// </summary>
        /// <returns></returns>
        public List<string> GetKey()
        {
            List<string> keys = new List<string>();
            keys.Add(Convert.ToBase64String(aes.Key));
            keys.Add(Convert.ToBase64String(aes.IV));
            return keys;
        }

        /// <summary>
        /// Sets the current key
        /// </summary>
        /// <param name="theKeys"></param>
        public void SetKey(List<string> theKeys)
        {
            aes.Key = Convert.FromBase64String(theKeys[0]);
            aes.IV = Convert.FromBase64String(theKeys[1]);
        }

        /// <summary>
        /// Ciphers the string using the current key
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public byte[] Cipher(string plainText)
        {
            // Key must have been set
            byte[] encrypted;
            // Create a new MemoryStream object to contain the encrypted bytes.
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Create a CryptoStream object to perform the encryption.
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
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

        /// <summary>
        /// Deciphers the string using the current key
        /// </summary>
        /// <param name="cipherBytes"></param>
        /// <returns></returns>
        public string DeCipher(byte[] cipherBytes)
        {
            // Key must have been set
            string decrypted;

            // Create a new MemoryStream object to contain the decrypted bytes.
            using (MemoryStream memoryStream = new MemoryStream(cipherBytes))
            {
                // Create a CryptoStream object to perform the decryption.
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    // Decrypt the ciphertext.
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        try
                        {
                            decrypted = streamReader.ReadToEnd();
                        }
                        catch
                        {
                            return "DeCiphering failed";
                        }
                    }

                }
            }

            return decrypted;
        }
    }
}
