using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using VdrioEForms.Security;
using VdrioEForms.UserManagement;

namespace VdrioEForms.EFForms
{
    [JsonObject]
    public class EFForm:TableEntity
    {
        [JsonIgnore]
        [IgnoreProperty]
        public List<EFEntry> Entries { get { return entries; } set { entries = value; EntriesJson = JsonConvert.SerializeObject(entries); } }
        [JsonIgnore]
        private List<EFEntry> entries;
        [IgnoreProperty]
        public string EntriesJson { get; set; }
        [JsonIgnore]
        public string EncryptedEntries { get; set; }
        [JsonIgnore]
        [IgnoreProperty]
        public List<EFEntry> DeletedEntries { get; set; }
        [JsonIgnore]
        public string EncryptedDeletedEntries { get; set; }
        [IgnoreProperty]
        public string FormName { get; set; }
        [JsonIgnore]
        public string EncryptedFormName { get; set; }
        [IgnoreProperty]
        public string TableName { get; set; }
        [JsonIgnore]
        public string EncryptedTableName { get; set; }
        [IgnoreProperty]
        public DateTime Created { get; set; }
        [JsonIgnore]
        public string EncryptedCreated { get; set; }
        [IgnoreProperty]
        public DateTime LastModified { get; set; }
        [JsonIgnore]
        public string EncryptedLastModified { get; set; }
        [IgnoreProperty]
        public EFUser OriginalUser { get; set; }
        [JsonIgnore]
        public string EncryptedOriginalUser { get; set; }
        [IgnoreProperty]
        public EFUser LastModifiedUser { get; set; }
        [JsonIgnore]
        public string EncryptedLastModifiedUser { get; set; }
        public string Initializor { get; set; }
        public bool Deleted { get; set; }
        public long TimeInTicks { get; set; }


        public EFForm()
        {
            Initializor = AESEncryptor.CreateInitializor();
            Entries = new List<EFEntry>();
            DeletedEntries = new List<EFEntry>();
            RowKey = Guid.NewGuid().ToString();

        }

        public void EncryptForm()
        {
            EncryptedEntries = AESEncryptor.EncryptTo64String(JsonConvert.SerializeObject(Entries), Convert.FromBase64String(Initializor));
            EncryptedDeletedEntries = AESEncryptor.EncryptTo64String(JsonConvert.SerializeObject(DeletedEntries), Convert.FromBase64String(Initializor));
            EncryptedFormName = AESEncryptor.EncryptTo64String(FormName, Convert.FromBase64String(Initializor));
            EncryptedTableName = AESEncryptor.EncryptTo64String(TableName, Convert.FromBase64String(Initializor));
            EncryptedCreated = AESEncryptor.EncryptTo64String(Created.ToString(), Convert.FromBase64String(Initializor));
            EncryptedLastModified = AESEncryptor.EncryptTo64String(LastModified.ToString(), Convert.FromBase64String(Initializor));
            EncryptedOriginalUser = AESEncryptor.EncryptTo64String(JsonConvert.SerializeObject(OriginalUser), Convert.FromBase64String(Initializor));
            EncryptedLastModifiedUser = AESEncryptor.EncryptTo64String(JsonConvert.SerializeObject(LastModifiedUser), Convert.FromBase64String(Initializor));
        }

        public void DecryptForm()
        {
            if (!string.IsNullOrEmpty(EncryptedEntries))
                Entries = JsonConvert.DeserializeObject<List<EFEntry>>(AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedEntries), Convert.FromBase64String(Initializor)));
            if (!string.IsNullOrEmpty(EncryptedDeletedEntries))
                DeletedEntries = JsonConvert.DeserializeObject<List<EFEntry>>(AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedDeletedEntries), Convert.FromBase64String(Initializor)));
            if (!string.IsNullOrEmpty(EncryptedFormName))
                FormName = AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedFormName), Convert.FromBase64String(Initializor));
            if (!string.IsNullOrEmpty(EncryptedTableName))
                TableName = AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedTableName), Convert.FromBase64String(Initializor));
            if (!string.IsNullOrEmpty(EncryptedCreated))
            {
                DateTime.TryParse(AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedCreated), Convert.FromBase64String(Initializor)), out DateTime created);
                Created = created;
            }
            if (!string.IsNullOrEmpty(EncryptedLastModified))
            {
                DateTime.TryParse(AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedLastModified), Convert.FromBase64String(Initializor)), out DateTime lastModified);
                LastModified = lastModified;
            }
            if (!string.IsNullOrEmpty(EncryptedOriginalUser))
                OriginalUser = JsonConvert.DeserializeObject<EFUser>(AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedOriginalUser), Convert.FromBase64String(Initializor)));
            if (!string.IsNullOrEmpty(EncryptedLastModifiedUser))
                LastModifiedUser = JsonConvert.DeserializeObject<EFUser>(AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedLastModifiedUser), Convert.FromBase64String(Initializor)));

        }

    }

    
}
