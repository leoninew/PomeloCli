using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace PomeloCli.DemoApp.Configuration
{
    class ApplicationProfile
    {
        private const String UseConfigurationFile = "appsettings.user.json";
        // private const String UseConfigurationFile = "appsettings.user.ini";

        public static string GetProfileDirectory()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var assemblyName = Assembly.GetEntryAssembly().GetName();
            return Path.Combine(userProfile, assemblyName.Name.ToLower());
        }

        public static String GetConfigurationFile()
        {
            return Path.Combine(GetProfileDirectory(), UseConfigurationFile);
        }

        public static IConfiguration GetConfiguration()
        {
            var useSettingFile = GetConfigurationFile();
            var configurationBuilder = new ConfigurationBuilder();

            if (File.Exists(useSettingFile))
            {
                configurationBuilder.AddJsonFile(useSettingFile);
                //configurationBuilder.AddIniFile(useSettingFile);
            }
            return configurationBuilder.Build();
        }

        public static void SaveConfiguration(IEnumerable<KeyValuePair<String, String>> settings)
        {
            var useSettingFile = GetConfigurationFile();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(useSettingFile, optional: true)
                //.AddIniFile(useSettingFile)
                .AddInMemoryCollection(settings)
                .Build();

            //var builder = new StringBuilder();
            //var items = configuration.AsEnumerablePairs();
            //foreach (var item in items)
            //{
            //    builder.AppendLine($"{item.Key}={item.Value}");
            //}
            //File.WriteAllText(useSettingFile, builder.ToString());

            var json = JsonConvert.SerializeObject(configuration.AsJson(), Formatting.Indented);
            File.WriteAllText(useSettingFile, json);
        }
    }
}