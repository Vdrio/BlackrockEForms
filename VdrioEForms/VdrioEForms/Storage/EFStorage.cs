using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(settingsFile);
                }
                else if (!Directory.Exists(Path.Combine(appDataBasePath, "Settings")))
                {
                    Directory.CreateDirectory(Path.Combine(appDataBasePath, "Settings"));
                }
                Settings settings = new Settings {  };
                System.IO.File.WriteAllText(Path.Combine(appDataBasePath, "Settings", "settings.txt"), Newtonsoft.Json.JsonConvert.SerializeObject(settings));
                string settingsJson = Newtonsoft.Json.JsonConvert.SerializeObject(settings);
                System.Diagnostics.Debug.WriteLine("Created new:" + settingsJson);
                return settings;
            }
            catch
            {
                return new Settings { UserEntry = "caught" };
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
                string settingsJson = Newtonsoft.Json.JsonConvert.SerializeObject(settings);
                System.IO.File.WriteAllText(Path.Combine(appDataBasePath, "Settings", "settings.txt"), settingsJson);
                return true;
            }
            catch { return false; }
        }
    }
}
