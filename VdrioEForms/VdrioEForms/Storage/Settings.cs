using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using VdrioEForms.EFForms;
using VdrioEForms.UserManagement;

namespace VdrioEForms.Storage
{
    public class Settings
    {
        public string UserEntry { get; set; }
        public string Password { get; set; }
        public bool SaveLoginInfo { get; set; }
        [JsonIgnore] 
        public EFUser LastUser { get { return lastUser; } set { lastUser = value;  LastUserJson = JsonConvert.SerializeObject(lastUser); } }
        [JsonIgnore]
        private EFUser lastUser;
        public string LastUserJson { get; set; }
        [JsonIgnore]
        public List<EFForm> SavedMainForms { get { return savedMainForms; } set { savedMainForms = value; SavedMainFormsJson = JsonConvert.SerializeObject(savedMainForms); } }
        [JsonIgnore]
        private List<EFForm> savedMainForms;
        public string SavedMainFormsJson { get; set; }
        [JsonIgnore]
        public List<EFForm> PendingForms { get { return pendingForms; } set { pendingForms = value; PendingFormsJson = JsonConvert.SerializeObject(pendingForms); } }
        [JsonIgnore]
        private List<EFForm> pendingForms;
        public string PendingFormsJson { get; set; }
    }
}
