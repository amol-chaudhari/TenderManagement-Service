using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace TenderManagement_Service.Test
{
    public static class StringExtensionMethods
    {
        private static Byte[] pKey
        {
            get
            {
                //CGafDKwC53sBX1EXmqjjmehIXoNqwtKQDgHl7UKL9
                return new Byte[]
                {
                    0x43, 0x47, 0x61, 0x66, 0x44, 0x4b, 0x77, 0x43, 0x35, 0x33, 0x73, 0x42, 0x58, 0x31, 0x45, 0x58, 0x6d,
                    0x71, 0x6a, 0x6a, 0x6d, 0x65, 0x68, 0x49, 0x58, 0x6f, 0x4e, 0x71, 0x77, 0x74, 0x4b, 0x51, 0x44, 0x67,
                    0x48, 0x6c, 0x37, 0x55, 0x4b, 0x4c, 0x39
                };
            }
        }

        private static Byte[] sKey
        {
            get
            {
                //BsHPTDLkDWgDyRIRp4uOLQxen3nKsSBqWWWZbqXeF8UW
                return new Byte[]
                {
                    0x42, 0x73, 0x48, 0x50, 0x54, 0x44, 0x4c, 0x6b, 0x44, 0x57, 0x67, 0x44, 0x79, 0x52, 0x49, 0x52, 0x70,
                    0x34, 0x75, 0x4f, 0x4c, 0x51, 0x78, 0x65, 0x6e, 0x33, 0x6e, 0x4b, 0x73, 0x53, 0x42, 0x71, 0x57, 0x57,
                    0x57, 0x5a, 0x62, 0x71, 0x58, 0x65, 0x46, 0x38, 0x55, 0x57
                };
            }
        }

        public static string Encrypt(this string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(pKey, sKey, 997);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(this string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(pKey, sKey, 997);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}