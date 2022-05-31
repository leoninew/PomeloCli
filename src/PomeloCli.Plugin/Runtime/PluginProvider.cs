using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Extensions.Options;

namespace PomeloCli.Plugin.Runtime {
    class PluginProvider : IPluginProvider {
        private readonly IOptions<PluginOptions> _pluginOptions;

        public PluginProvider(IOptions<PluginOptions> pluginOptions) {
            _pluginOptions = pluginOptions;
        }

        public String GetPluginDir(Boolean createIfNotExist) {
            var projectDir = _pluginOptions.Value.ProjectDir;
            if (String.IsNullOrWhiteSpace(projectDir) == false && Directory.GetParent(projectDir).Exists) {
                if (createIfNotExist && Directory.Exists(projectDir) == false) {
                    Directory.CreateDirectory(projectDir);
                }

                return projectDir;
            }

            var assemblyName = Assembly.GetEntryAssembly().GetName();
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            projectDir = Path.Combine(userProfile, assemblyName.Name.ToLower());
            if (createIfNotExist && Directory.Exists(projectDir) == false) {
                Directory.CreateDirectory(projectDir);
            }

            return projectDir;
        }

        public String GetPluginCsproj(Boolean createIfNotExist) {
            var projectDir = GetPluginDir(createIfNotExist);
            var assemblyName = Assembly.GetEntryAssembly().GetName();
            var pluginCsproj = Path.Combine(projectDir, assemblyName.Name + ".Plugin.csproj");
            if (createIfNotExist == false) {
                return pluginCsproj;
            }

            var template = Environment.Version.Major switch {
                3 => Resources.netcoreapp3_1,
                5 or 6 => Resources.net5_0,
                _ => throw new ArgumentException($"dotnet version '{Environment.Version}' not supported")
            };
            if (File.Exists(pluginCsproj) == false) {
                File.WriteAllText(pluginCsproj, template);
            }

            return pluginCsproj;
        }

        public IEnumerable<Plugin> FindAll() {
            var packages = GetPackageElements();
            foreach (var item in packages) {
                yield return new Plugin {
                    Name = item.Attribute("Include")?.Value,
                    Version = item.Attribute("Version")?.Value,
                    Source = item.Attribute("Source")?.Value,
                    Assembly = item.Attribute("Assembly")?.Value
                };
            }
        }

        public void Save(Plugin plugin) {
            var package = GetPackageElements()
                .FirstOrDefault(x => plugin.Name.Equals(x.Attribute("Include")?.Value, StringComparison.OrdinalIgnoreCase));
            if (package == null) {
                return;
            }

            if (plugin.Source != null) {
                package.SetAttributeValue("Source", plugin.Source);
            }

            if (plugin.Assembly != null) {
                package.SetAttributeValue("Assembly", plugin.Assembly);
            }

            if (package.Document != null) {
                var pluginCsproj = GetPluginCsproj(true);
                package.Document.Save(pluginCsproj);
            }
        }

        private IEnumerable<XElement> GetPackageElements() {
            var pluginCsproj = GetPluginCsproj(false);
            if (File.Exists(pluginCsproj) == false) {
                return Enumerable.Empty<XElement>();
            }

            return XDocument.Load(pluginCsproj)
                .DescendantNodes()
                .OfType<XElement>()
                .Where(x => x.Name == "PackageReference");
        }
    }
}