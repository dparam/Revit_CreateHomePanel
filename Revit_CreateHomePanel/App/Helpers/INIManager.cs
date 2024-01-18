using Autodesk.Revit.DB.Macros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace Revit_CreateHomePanel.App.Helpers
{
    public class INIManager
    {
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        private const int SIZE = 1024;
        private string path = null;

        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]

        private static extern int GetPrivateString(string section, string key, string def, StringBuilder buffer, int size, string path);


        public INIManager(string aPath)
        {
            path = aPath;
        }


        public string GetPrivateString(string aSection, string aKey)
        {
            StringBuilder buffer = new StringBuilder(SIZE);
            GetPrivateString(aSection, aKey, null, buffer, SIZE, path);

            return buffer.ToString();
        }
    }
}
