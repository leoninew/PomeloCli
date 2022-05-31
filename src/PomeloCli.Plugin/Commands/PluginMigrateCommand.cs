using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;
using PomeloCli.Attributes;
using PomeloCli.Plugin.Runtime;

namespace PomeloCli.Plugin.Commands {
    [Command("migrate")]
    class PluginMigrateCommand : Command<PluginCommand> {
        private readonly IOptions<PluginOptions> _pluginOptions;
        private readonly IPluginProvider _pluginProvider;

        public PluginMigrateCommand(IPluginProvider pluginProvider, IOptions<PluginOptions> pluginOptions) {
            _pluginProvider = pluginProvider;
            _pluginOptions = pluginOptions;
        }

        [CommandOption("-v|--version", CommandOptionType.SingleValue,
            Description = "The version of the package to add.")]
        public String Version { get; set; }

        public override Int32 Execute() {
            var pluginCsproj = _pluginProvider.GetPluginCsproj(false);
            if (File.Exists(pluginCsproj) == false) {
                return 0;
            }

            _pluginProvider.GetPluginCsproj(true); // ensure initialized
            var pluginDir = _pluginProvider.GetPluginDir(false);
            var packageDir = _pluginOptions.Value.GetPackageDir();

            var plugins = _pluginProvider.FindAll().ToArray();
            foreach (var plugin in plugins) {
                var dotnetPackage = new DotnetPackage(plugin.Name, Version, plugin.Source);
                DotnetPackageManager.AddPackage(dotnetPackage, pluginDir, packageDir);

                _pluginProvider.Save(new Plugin {
                    Name = plugin.Name,
                    Version = Version,
                    Source = plugin.Source,
                    Assembly = plugin.Assembly
                });
            }

            return 0;
        }
    }
}