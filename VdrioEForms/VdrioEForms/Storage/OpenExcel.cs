using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VdrioEForms.Storage
{
    public class OpenExcel
    {
        public static void OpenExcelDoc(string fileName)
        {
            App.openExcel.OpenExcelDoc(fileName);
        }

        public static Task<string> CreateAndroidExternalFile(string shortFileName)
        {
            return App.openExcel.CreateExternalFile(shortFileName);
        }
    }
}
