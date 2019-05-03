using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace VdrioEForms.Security
{
    public static class AESEncryptor
    {
        static bool Initialized = false;
        static AesManaged EncryptionManager;
        static ICryptoTransform Encryptor;
        static readonly string keyString = "KyvNjApj2/p8cPt6nmrQXK4XXpjZLKIUHAgTwLmTpbg=";
        static RandomNumberGenerator NumberGenerator;

        public static void Initialize()
        {
            EncryptionManager = new AesManaged();
            EncryptionManager.Key = Convert.FromBase64String(keyString);
            NumberGenerator = new RNGCryptoServiceProvider();
            Initialized = true;
            Debug.WriteLine("Use this Key: " + Convert.ToBase64String(EncryptionManager.Key));
        }

        public static string EncryptTo64String(string plainText, byte[] iv)
        {
            byte[] encrypted = Encrypt(plainText, iv);
            return Convert.ToBase64String(encrypted);
        }
        public static byte[] Encrypt(string plainText, byte[] iv)
        {
            if (!Initialized)
            {
                Initialize();
            }
            Encryptor = EncryptionManager.CreateEncryptor(EncryptionManager.Key, iv);
            byte[] encrypted;
            //byte[] data = Encoding.UTF8.GetBytes(plainText);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, Encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    encrypted = ms.ToArray();
                }
            }
            return encrypted;
        }

        public static string Decrypt(byte[] cipherText, byte[] iv)
        {
            if (!Initialized)
            {
                Initialize();
            }
            string plaintext = null;
            // Create AesManaged    
            
                // Create a decryptor    
                ICryptoTransform decryptor = EncryptionManager.CreateDecryptor(EncryptionManager.Key, iv);
            // Create the streams used for decryption.    
            using (MemoryStream ms = new MemoryStream(cipherText))
            {
                // Create crypto stream    
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    // Read crypto stream    
                    using (StreamReader reader = new StreamReader(cs))
                    {
                        plaintext = reader.ReadToEnd();
                    }
                        
                }
            }
            
            return plaintext;
        }

        public static string CreateInitializor()
        {
            if (!Initialized)
            {
                Initialize();
            }
            EncryptionManager.GenerateIV();
            return Convert.ToBase64String(EncryptionManager.IV);
        }
    }
}
