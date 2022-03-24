using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;
using PomeloCli.Attributes;
using PomeloCli.Plugin.Runtime;

namespace PomeloCli.Plugin.Commands {
    [Command("install")]
    class PluginInstallCommand : Command<PluginCommand> {
        private readonly IOptions<PluginOptions> _pluginOptions;
        private readonly IPluginProvider _pluginProvider;

        public PluginInstallCommand(IOptions<PluginOptions> pluginOptions, IPluginProvider pluginProvider) {
            _pluginProvider = pluginProvider;
            _pluginOptions = pluginOptions;
        }

        [Required]
        [CommandArgument("name", false,
            Description = "The package reference to add.")]
        public String Name { get; set; }

        [CommandOption("-v|--version", CommandOptionType.SingleValue,
            Description = "The version of the package to add.")]
        public String Version { get; set; }

        [CommandOption("-s|--source", CommandOptionType.SingleValue,
            Description = "The NuGet package source to use during the restore.")]
        public String Source { get; set; }

        [CommandOption("-a|--assembly", CommandOptionType.SingleValue)]
        public String Assembly { get; set; }

        public override Int32 Execute() {
            var dotnetPackage = new DotnetPackage(Name, Version, Source);
            _pluginProvider.GetPluginCsproj(true); // ensure initialized
            var pluginDir = _pluginProvider.GetPluginDir(false);
            var packageDir = _pluginOptions.Value.GetPackageDir();
            DotnetPackageManager.AddPackage(dotnetPackage, pluginDir, packageDir);

            _pluginProvider.Save(new Plugin {
                Name = Name,
                Version = Version,
                Source = Source,
                Assembly = Assembly
            });

            return 0;
        }
    }
}