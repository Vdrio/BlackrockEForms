using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VdrioEForms.EFForms;
using VdrioEForms.UserManagement;
using Xamarin.Forms;

namespace VdrioEForms.Storage
{
    public class EFStorage : IEFStorage
    {
        public static string appDataBasePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);

        public EFStorage()
        {
            CreateDirectoryIfNotExists("Files");
            if (Device.RuntimePlatform == "Android")
            {
                CreateExternalForAndroid();
            }
        }

        public void SetupStorage()
        {
            CreateAndOrOpenSettings();
            CreateDirectoryIfNotExists("Files");
        }

        public Settings CreateAndOrOpenSettings()
        {
            try
            {
                if (Directory.Exists(Path.Combine(appDataBasePath, "Settings"))
                    && System.IO.File.Exists(Path.Combine(appDataBasePath, "Settings", "settings.txt")))
                {
                    string settingsFile = System.IO.File.ReadAllText(Path.Combine(appDataBasePath, "Settings", "settings.txt"));
                    var setngs = Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(settingsFile);
                    if (!string.IsNullOrEmpty(setngs.PendingFormsJson))
                        setngs.PendingForms = JsonConvert.DeserializeObject<List<EFForm>>(setngs.PendingFormsJson);
                    if (!string.IsNullOrEmpty(setngs.SavedMainFormsJson))
                        setngs.SavedMainForms = JsonConvert.DeserializeObject<List<EFForm>>(setngs.SavedMainFormsJson);
                    if (!string.IsNullOrEmpty(setngs.LastUserJson))
                    {
                        setngs.LastUser = JsonConvert.DeserializeObject<EFUser>(setngs.LastUserJson);
                        setngs.LastUser.DecryptUser();
                    }
                    if (setngs.PendingForms == null)
                    {
                        setngs.PendingForms = new List<EFForm>();
                    }
                    else
                    {
                        foreach (EFForm f in setngs.PendingForms)
                        {
                            f.Entries = JsonConvert.DeserializeObject<List<EFEntry>>(f.EntriesJson);
                            foreach (EFEntry e in f.Entries)
                            {
                                e.DecryptEntry();
                            }
                        }
                    }
                    if (setngs.SavedMainForms == null)
                    {
                        setngs.SavedMainForms = new List<EFForm>();
                    }
                    else
                    {
                        foreach (EFForm f in setngs.SavedMainForms)
                        {
                            f.Entries = JsonConvert.DeserializeObject<List<EFEntry>>(f.EntriesJson);
                            foreach (EFEntry e in f.Entries)
                            {
                                e.DecryptEntry();
                            }
                        }
                    }
                    return setngs;
                }
                else if (!Directory.Exists(Path.Combine(appDataBasePath, "Settings")))
                {
                    Directory.CreateDirectory(Path.Combine(appDataBasePath, "Settings"));
                }
                Settings settings = new Settings { PendingForms = new List<EFForm>(), SavedMainForms = new List<EFForm>() };
                System.IO.File.WriteAllText(Path.Combine(appDataBasePath, "Settings", "settings.txt"), Newtonsoft.Json.JsonConvert.SerializeObject(settings));
                string settingsJson = Newtonsoft.Json.JsonConvert.SerializeObject(settings);
                System.Diagnostics.Debug.WriteLine("Created new:" + settingsJson);

                return settings;
            }
            catch
            {
                return new Settings { PendingForms = new List<EFForm>(), SavedMainForms = new List<EFForm>() };
            }
        }

        public void CreateDirectoryIfNotExists(string DirectoryName)
        {
            if (!Directory.Exists(Path.Combine(appDataBasePath,DirectoryName)))
            {
                Directory.CreateDirectory(Path.Combine(appDataBasePath, DirectoryName));

            }
        }

        public string UploadFile( string fileName, byte[] data, string directoryName = "Files")
        {
            string fullName;
            fullName = Path.Combine(appDataBasePath, directoryName, fileName);
            CreateDirectoryIfNotExists(directoryName);
            File.WriteAllBytes(fullName, data);
            return fullName;
        }

        public string CreateFile(string fileName)
        {
            if (Device.RuntimePlatform == "Android")
            {
                OpenExcel.CreateAndroidExternalFile(fileName);
            }
            else
            {
                Debug.WriteLine("Creating file");
                File.Create(fileName).Dispose();
                Debug.WriteLine("Created file");
            }
            
            return fileName;
        }

        void CreateExternalForAndroid()
        {
            
            
        }



        public bool UpdateSettings(Settings settings)
        {
            try
            {
                if (settings.PendingForms != null)
                {
                    foreach(EFForm f in settings.PendingForms)
                    {
                        foreach(EFEntry e in f.Entries)
                        {
                            e.EncryptEntry();
                        }
                        f.EntriesJson = JsonConvert.SerializeObject(f.Entries);
                        f.EncryptForm();
                    }
                }
                if (settings.SavedMainForms != null)
                {
                    foreach (EFForm f in settings.SavedMainForms)
                    {
                        foreach (EFEntry e in f.Entries)
                        {
                            e.EncryptEntry();
                        }
                        f.EntriesJson = JsonConvert.SerializeObject(f.Entries);
                        f.EncryptForm();
                    }
                }
                settings.PendingFormsJson = JsonConvert.SerializeObject(settings.PendingForms);
                settings.SavedMainFormsJson = JsonConvert.SerializeObject(settings.SavedMainForms);
                settings.LastUser.EncryptUser();
                settings.LastUserJson = JsonConvert.SerializeObject(settings.LastUser);
                string settingsJson = Newtonsoft.Json.JsonConvert.SerializeObject(settings);
                System.IO.File.WriteAllText(Path.Combine(appDataBasePath, "Settings", "settings.txt"), settingsJson);
                return true;
            }
            catch { return false; }
        }
    }
}
