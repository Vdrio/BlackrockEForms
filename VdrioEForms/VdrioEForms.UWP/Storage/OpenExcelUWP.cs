using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VdrioEForms.Storage;
using Windows.Storage;
using Windows.System;

namespace VdrioEForms.UWP.Storage
{
    public class OpenExcelUWP : IOpenExcel
    {
        public Task<string> CreateExternalFile(string shortFileName)
        {
            throw new NotImplementedException();
        }

        public async void OpenExcelDoc(string fileName)
        {
            Windows.Storage.StorageFile file = await (await ApplicationData.Current.LocalFolder.GetFolderAsync("Files")).GetFileAsync(fileName);
            LauncherOptions options = new LauncherOptions { DisplayApplicationPicker = true };
            //Debug.WriteLine(await Launcher.LaunchUriAsync(new Uri(fileName), options));
            await Launcher.LaunchFileAsync(file, options);
        }
    }
}
