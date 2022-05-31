using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace PomeloCli.DemoApp.Configuration {
    public static class ConfigurationExtensions {
        public static JObject AsJson(this IConfiguration configuration) {
            var jobject = new JObject();
            foreach (var section in configuration.GetChildren()) {
                AsJsonProperty(section, jobject);
            }

            return jobject;
        }

        private static void AsJsonProperty(IConfigurationSection section, JObject jobject) {
            if (section.Value == null) {
                // 重要: 注意引用关系, 不可拆分
                jobject.Add(section.Key, jobject = new JObject());
                foreach (var children in section.GetChildren()) {
                    AsJsonProperty(children, jobject);
                }
            }
            else {
                jobject.Add(section.Key, section.Value);
            }
        }

        public static IEnumerable<KeyValuePair<String, String>> AsEnumerablePairs(this IConfiguration configuration) {
            return configuration.AsEnumerable(false).Where(x => x.Value != null);
        }

        public static IConfigurationBuilder AddApplicationProfile(this IConfigurationBuilder builder) {
            // builder.AddInMemoryCollection(ApplicationProfile.GetConfiguration().AsEnumerablePairs());
            builder.AddJsonFile(ApplicationProfile.GetConfigurationFile(), true);
            return builder;
        }
    }
}