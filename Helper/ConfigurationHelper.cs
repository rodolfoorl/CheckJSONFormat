using Newtonsoft.Json;
using System.IO;

namespace CheckJSONFormat.Helper
{
    public static class ConfigurationHelper
    {
        readonly static string PathConfig = $"{Directory.GetCurrentDirectory()}\\appsettings.json";

        public static void CreateDefaultConfig(ref AppSettings appSettings)
        {
            appSettings = new AppSettings();

            if (File.Exists(PathConfig))
            {
                var jsonContent = File.ReadAllText(PathConfig);
                appSettings = JsonConvert.DeserializeObject<AppSettings>(jsonContent);
            }
        }
    }
}
