using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VdrioEForms.Storage
{
    public interface IOpenExcel
    {
        void OpenExcelDoc(string fileName);
        Task<string> CreateExternalFile(string shortFileName);
    }
}
