using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using VdrioEForms.Security;

namespace VdrioEForms.UserManagement
{
    public class EFUser:TableEntity
    {
        [JsonIgnore][IgnoreProperty]
        public string UserName { get; set; }
        public string EncryptedUserName { get; set; }
        [JsonIgnore] [IgnoreProperty]
        public string Email { get; set; }
        public string EncryptedEmail { get; set; }
        [JsonIgnore] [IgnoreProperty]
        public string FirstName { get; set; }
        public string EncryptedFirstName { get; set; }
        [JsonIgnore] [IgnoreProperty]
        public string LastName { get; set; }
        public string EncryptedLastName { get; set; }
        [JsonIgnore][IgnoreProperty]
        public string Password { get; set; }
        public string EncryptedPassword { get; set; }
        public int UserType { get; set; }
        public bool Active { get; set; } = true;
        public string Initializor { get; set; }
        public bool Deleted { get; set; }

        public EFUser()
        {
            Initializor = AESEncryptor.CreateInitializor();
        }

        public void EncryptUser()
        {
            EncryptedUserName = AESEncryptor.EncryptTo64String(UserName, Convert.FromBase64String(Initializor));
            EncryptedEmail = AESEncryptor.EncryptTo64String(Email, Convert.FromBase64String(Initializor));
            EncryptedPassword = AESEncryptor.EncryptTo64String(Password, Convert.FromBase64String(Initializor));
            EncryptedFirstName = AESEncryptor.EncryptTo64String(FirstName, Convert.FromBase64String(Initializor));
            EncryptedLastName = AESEncryptor.EncryptTo64String(LastName, Convert.FromBase64String(Initializor));
        }

        public void DecryptUser()
        {
            
            UserName = AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedUserName), Convert.FromBase64String(Initializor));
            Email = AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedEmail), Convert.FromBase64String(Initializor));
            Password = AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedPassword), Convert.FromBase64String(Initializor));
            FirstName = AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedFirstName), Convert.FromBase64String(Initializor));
            LastName = AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedLastName), Convert.FromBase64String(Initializor));
        }
    }
}
