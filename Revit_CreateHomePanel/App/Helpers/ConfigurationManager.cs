using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;


namespace Revit_CreateHomePanel.App.Helpers
{
    public static class ConfigurationManager
    {
        public static void SavePluginConfiguration(Dictionary<string, string> dict, string pluginName)
        {
            string tempPath = Path.GetTempPath();
            string path = $"{tempPath}{pluginName}.txt";

            File.WriteAllText(path, new JavaScriptSerializer().Serialize(dict));
        }


        public static Dictionary<string, string> LoadPluginConfiguration(string pluginName)
        {
            string tempPath = Path.GetTempPath();
            string path = $"{tempPath}{pluginName}.txt";

            if (!File.Exists(path)) return null;

            JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
            Dictionary<string, string> dictionary = scriptSerializer.Deserialize<Dictionary<string, string>>(
                File.ReadAllText($"{tempPath}{pluginName}.txt")
                );

            return dictionary;
        }
    }
}
