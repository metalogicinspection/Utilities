using System.IO;
using System.Security.Cryptography;

namespace DropboxUploader.Core
{
    internal class AESEncryption
    {

        public static byte[] Encrypt(byte[] src, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;

                aes.IV = iv;

                using (var ms = new MemoryStream())

                using (var cstream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {

                    cstream.Write(src, 0, src.Length);

                    cstream.Close();

                    ms.Close();

                    return ms.ToArray();
                }
            }
        }
        
        public static byte[] Decrypt(byte[] src, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;

                aes.IV = iv;

                using (var ms = new MemoryStream(src))

                using (var destMs = new MemoryStream())

                using (var cstream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    var data = new byte[100];

                    int readLen;
                    while ((readLen = cstream.Read(data, 0, 100)) > 0)
                    {
                        destMs.Write(data, 0, readLen);
                    }
                    return destMs.ToArray();
                }
            }
        }

    }
}