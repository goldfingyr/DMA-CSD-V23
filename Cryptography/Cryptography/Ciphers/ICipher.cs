namespace Cryptography.Ciphers
{
    public interface ICipher
    {
        public List<string> GetKey();
        public void SetKey(List<string> theKeys);
        public byte[] Cipher(string plainText);
        public string DeCipher(byte[] cipherBytes);
    }
}
