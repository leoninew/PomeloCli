using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.Extensions.Options;
using PomeloCli.Attributes;
using PomeloCli.Plugin.Runtime;

namespace PomeloCli.Plugin.Commands {
    [Command("uninstall")]
    class PluginUninstallCommand : Command<PluginCommand> {
        private readonly IOptions<PluginOptions> _pluginOptions;
        private readonly IPluginProvider _pluginProvider;

        public PluginUninstallCommand(IOptions<PluginOptions> pluginOptions, IPluginProvider pluginProvider) {
            _pluginProvider = pluginProvider;
            _pluginOptions = pluginOptions;
        }

        [Required]
        [CommandArgument("name", false,
            Description = "The package reference to remove.")]
        public String Name { get; set; }

        public override Int32 Execute() {
            var pluginCsproj = _pluginProvider.GetPluginCsproj(false);
            if (File.Exists(pluginCsproj) == false) {
                return 0;
            }

            _pluginProvider.GetPluginCsproj(true); // ensure initialized
            var pluginDir = _pluginProvider.GetPluginDir(false);
            DotnetPackageManager.RemovePackage(Name, pluginDir);
            return 0;
        }
    }
}