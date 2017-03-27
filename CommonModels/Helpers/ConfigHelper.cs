using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonModels.Helpers
{
    public static class ConfigHelper
    {
        private static Configuration GetConfiguration(string configFileName)
        {
            string appPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configFile = System.IO.Path.Combine(appPath, configFileName);
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;
            return ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        }

        public static void UpdateConfig(string configFileName, string settingKey, string value)
        {
            Configuration config = GetConfiguration(configFileName);
            config.AppSettings.Settings[settingKey].Value = value;
            config.Save();
        }

        public static Dictionary<string, string> GetAppSettings(string configFileName)
        {
            Configuration config = GetConfiguration(configFileName);
            Dictionary<string, string> settings = new Dictionary<string, string>();
            foreach (KeyValueConfigurationElement appSetting in config.AppSettings.Settings)
            {
                settings.Add(appSetting.Key, appSetting.Value);
            }
            return settings;
        }

        public static string GetConnectionString(string configFileName, string connectionName)
        {
            Configuration config = GetConfiguration(configFileName);
            return config.ConnectionStrings.ConnectionStrings[connectionName].ConnectionString;
        }

        public static string GetAppSetting(string configFileName, string key)
        {
            Configuration config = GetConfiguration(configFileName);
            return config.AppSettings.Settings[key].Value;
        }
    }
}
