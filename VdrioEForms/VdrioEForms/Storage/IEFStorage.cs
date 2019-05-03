using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VdrioEForms.Storage
{
    public interface IEFStorage
    {

        Settings CreateAndOrOpenSettings();
        bool UpdateSettings(Settings settings);


    }
}
