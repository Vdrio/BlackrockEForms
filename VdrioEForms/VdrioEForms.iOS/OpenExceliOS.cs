using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using Microsoft.WindowsAzure.Storage.Blob;
using UIKit;
using VdrioEForms.Azure;
using VdrioEForms.Storage;
using Xamarin.Forms;

namespace VdrioEForms.iOS
{
    public class OpenExceliOS : IOpenExcel
    {
        public Task<string> CreateExternalFile(string shortFileName)
        {
            throw new NotImplementedException();
        }

        public async void OpenExcelDoc(string fileName)
        {
            byte[] mydoc = System.IO.File.ReadAllBytes(fileName);
            string[] split = fileName.Split("/");
            string shortName = split[split.Length - 1];
            await AzureTableManager.SaveBlockBlob("documents", mydoc, shortName);
            var blobList = await AzureTableManager.GetBlobs<CloudBlockBlob>("documents");
            var myBlob = blobList.Find(b => b.Name == shortName);
            var originalString = @myBlob.Uri;
            //var encodedString = originalString.(withAllowedCharacters: .urlQueryAllowed)
            string google = @"https://docs.google.com/viewer?url=";
            var encodedURLString = @google + @originalString;
            System.Diagnostics.Debug.WriteLine(encodedURLString);
            encodedURLString = Uri.EscapeUriString(encodedURLString);
            System.Diagnostics.Debug.WriteLine(encodedURLString + "hello");
            Device.OpenUri(new Uri(encodedURLString));
        }
    }
}