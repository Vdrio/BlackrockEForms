using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using VdrioEForms.Security;

namespace VdrioEForms.EFForms
{
    public class EFEntry
    {
        [JsonIgnore]
        public string EntryName { get; set; }
        public string EncryptedEntryName { get; set; }
        [JsonIgnore]
        public string EntryData { get; set; }
        public string EncryptedEntryData { get; set; }
        [JsonIgnore]
        public string Units { get; set; }
        public string EncryptedUnits { get; set; }
        [JsonIgnore]
        public List<string> Selections { get; set; }
        public string EncryptedSelections { get; set; }
        public string EntryID { get; set; }
        public string Initializor { get; set; }
        public int EntryType { get; set; }
        public bool Deleted { get; set; }

        public EFEntry()
        {
            EntryID = Guid.NewGuid().ToString();
            Selections = new List<string>();
            Initializor = AESEncryptor.CreateInitializor();
        }

        public void EncryptEntry()
        {
            EncryptedEntryName = AESEncryptor.EncryptTo64String(EntryName, Convert.FromBase64String(Initializor));
            EncryptedEntryData = AESEncryptor.EncryptTo64String(EntryData, Convert.FromBase64String(Initializor));
            EncryptedUnits = AESEncryptor.EncryptTo64String(Units, Convert.FromBase64String(Initializor));
            EncryptedSelections = AESEncryptor.EncryptTo64String(JsonConvert.SerializeObject(Selections), Convert.FromBase64String(Initializor));
        }

        public void DecryptEntry()
        {
            Debug.WriteLine(EncryptedEntryData);
            if (EncryptedEntryName != null)
                EntryName = AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedEntryName), Convert.FromBase64String(Initializor));
            if (EncryptedEntryData != null)
                EntryData = AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedEntryData), Convert.FromBase64String(Initializor));
            if (EncryptedUnits != null)
                Units = AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedUnits), Convert.FromBase64String(Initializor));
            Selections = JsonConvert.DeserializeObject<List<string>>(AESEncryptor.Decrypt(Convert.FromBase64String(EncryptedSelections), Convert.FromBase64String(Initializor)));
        }
    }
}
