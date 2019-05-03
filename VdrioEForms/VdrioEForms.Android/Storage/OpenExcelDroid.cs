using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using VdrioEForms.Storage;
using Plugin.Permissions;
using System.Threading.Tasks;

namespace VdrioEForms.Droid.Storage
{
    public class OpenExcelDroid : IOpenExcel
    {
        string baseExternal = Android.OS.Environment.ExternalStorageDirectory.ToString();
        public async Task<string> CreateExternalFile(string shortFileName)
        {
            CreateDirectory("BlackrockLogisticsEForms", "Files");
            //var file = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments);
            if (await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage)
                == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                File.Create(Path.Combine(baseExternal, "BlackrockLogisticsEForms", "Files", shortFileName)).Dispose();
                return Path.Combine(baseExternal, "BlackrockLogisticsEForms", "Files", shortFileName);
            }
            else
            {
                Dictionary<Plugin.Permissions.Abstractions.Permission, Plugin.Permissions.Abstractions.PermissionStatus> granted = new Dictionary<Plugin.Permissions.Abstractions.Permission, Plugin.Permissions.Abstractions.PermissionStatus>();
                granted.Add(Plugin.Permissions.Abstractions.Permission.Storage, Plugin.Permissions.Abstractions.PermissionStatus.Granted);
                if (await CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage) == granted)
                {
                    File.Create(Path.Combine(baseExternal, "BlackrockLogisticsEForms", "Files", shortFileName)).Dispose();
                    return Path.Combine(baseExternal, "BlackrockLogisticsEForms", "Files", shortFileName);
                }
            }
            return "";
        }

        public string CreateDirectory(string directoryName, string folderName)
        {
            var directoryPath = Path.Combine(baseExternal, directoryName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            directoryPath = Path.Combine(directoryPath, folderName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return directoryPath;
        }


        public void OpenExcelDoc(string fileName)
        {
            Java.IO.File file = new Java.IO.File(fileName);
            Intent intent = new Intent(Intent.ActionView);
            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
            intent.SetDataAndType(Android.Net.Uri.FromFile(file), "application/vnd.ms-excel");
            //intent.SetData()
            MainActivity.activity.ApplicationContext.StartActivity(intent);
        }


    }
}