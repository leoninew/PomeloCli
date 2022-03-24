using System;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;
using PomeloCli.Attributes;
using PomeloCli.Plugin.Runtime;

namespace PomeloCli.Plugin.Commands {
    [Command("list")]
    class PluginListCommand : Command<PluginCommand> {
        private readonly IOptions<PluginOptions> _pluginOptions;
        private readonly IPluginProvider _pluginProvider;

        public PluginListCommand(IOptions<PluginOptions> pluginOptions, IPluginProvider pluginProvider) {
            _pluginProvider = pluginProvider;
            _pluginOptions = pluginOptions;
        }

        [CommandOption("-s|--source", CommandOptionType.SingleValue,
            Description = "The NuGet package source to use during the restore.")]
        public String Source { get; set; }

        public override Int32 Execute() {
            var pluginCsproj = _pluginProvider.GetPluginCsproj(false);
            if (File.Exists(pluginCsproj) == false) {
                return 0;
            }

            var pluginDir = _pluginProvider.GetPluginDir(false);
            DotnetPackageManager.ListPackage(Source, pluginDir);
            return 0;
        }
    }
}